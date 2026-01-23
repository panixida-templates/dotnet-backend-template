using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;

using Common.Constants;

using Dal.S3.Configurations;
using Dal.S3.Implementations.Core;
using Dal.S3.Interfaces.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Globalization;
using System.Reflection;

namespace Dal.S3.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseS3Storage(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IAmazonS3>(serviceProvider =>
        {
            var serviceUrl = configuration[AppsettingsKeysConstants.S3ServiceUrl];
            var region = configuration[AppsettingsKeysConstants.S3Region];
            var forcePathStyle = configuration.GetValue<bool>(AppsettingsKeysConstants.S3ForcePathStyle);

            var s3Config = new AmazonS3Config
            {
                ForcePathStyle = forcePathStyle,
                ServiceURL = serviceUrl,
                AuthenticationRegion = region,
            };

            var accessKey = configuration[AppsettingsKeysConstants.S3AccessKey];
            var secretKey = configuration[AppsettingsKeysConstants.S3SecretKey];

            if (!string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                return new AmazonS3Client(credentials, s3Config);
            }

            return new AmazonS3Client(s3Config);
        });

        serviceCollection.AddSingleton<ITransferUtility>(serviceProvider =>
        {
            var amazonS3 = serviceProvider.GetRequiredService<IAmazonS3>();
            return new TransferUtility(amazonS3);
        });

        serviceCollection.AddOptions<S3StorageOptions>()
            .Configure(options =>
            {
                options.ServiceUrl = configuration[AppsettingsKeysConstants.S3ServiceUrl]
                    ?? throw new InvalidOperationException($"{AppsettingsKeysConstants.S3ServiceUrl} не задан в конфигурации.");

                options.BucketName = configuration[AppsettingsKeysConstants.S3BucketName] 
                    ?? throw new InvalidOperationException($"{AppsettingsKeysConstants.S3BucketName} не задан в конфигурации.");

                options.BasePrefix = configuration[AppsettingsKeysConstants.S3BasePrefix] 
                    ?? throw new InvalidOperationException($"{AppsettingsKeysConstants.S3BasePrefix} не задан в конфигурации.");

                var ttlRaw = configuration[AppsettingsKeysConstants.S3DefaultPresignTtl];
                if (!TimeSpan.TryParse(ttlRaw, CultureInfo.InvariantCulture, out var ttl))
                {
                    throw new InvalidOperationException(
                        $"Некорректное значение '{AppsettingsKeysConstants.S3DefaultPresignTtl}': '{ttlRaw}'. " +
                        "Ожидается формат TimeSpan, например '00:15:00'.");
                }

                options.DefaultPresignTtl = ttl;
            });

        serviceCollection.AddSingleton(serviceProvider =>
        {
            return serviceProvider.GetRequiredService<IOptions<S3StorageOptions>>().Value;
        });

        serviceCollection.RegisterS3Storages(typeof(S3Storage).Assembly);

        return serviceCollection;
    }

    private static IServiceCollection RegisterS3Storages(this IServiceCollection serviceCollection, Assembly assembly)
    {
        foreach (var dalImplementation in assembly.GetTypes())
        {
            if (!dalImplementation.IsClass || dalImplementation.IsAbstract || dalImplementation.IsGenericTypeDefinition)
            {
                continue;
            }

            var dalInterfaces = dalImplementation.GetInterfaces()
                .Where(iface =>
                {
                    if (!iface.IsInterface || iface.IsGenericType || iface == typeof(IStorage))
                    {
                        return false;
                    }

                    return typeof(IStorage).IsAssignableFrom(iface);
                })
                .ToArray();

            foreach (var dalInterface in dalInterfaces)
            {
                if (serviceCollection.Any(descriptor =>
                {
                    return descriptor.ServiceType == dalInterface;
                }))
                {
                    throw new InvalidOperationException(
                        $"DAL интерфейс '{dalInterface.FullName}' уже зарегистрирован. " +
                        $"Конфликт реализации: '{dalImplementation.FullName}'.");
                }

                serviceCollection.AddScoped(dalInterface, dalImplementation);
            }
        }

        return serviceCollection;
    }
}
