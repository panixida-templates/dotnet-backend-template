using Application.Abstractions.Persistence;
using Application.Abstractions.Persistence.Read;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Infrastructure.Ef.Core.Bootstrapper;

/// <summary>
/// Регистрирует реализации инфраструктурных репозиториев из указанной сборки.
/// </summary>
internal static class RepositoryImplementationsRegistrar
{
    /// <summary>
    /// Находит в сборке реализации репозиториев и добавляет их в контейнер зависимостей.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов для регистрации.</param>
    /// <param name="assembly">Сборка с реализациями репозиториев.</param>
    /// <returns>Та же коллекция сервисов после регистрации.</returns>
    public static IServiceCollection Register(IServiceCollection serviceCollection, Assembly assembly)
    {
        foreach (var repositoryImplementation in assembly.GetTypes())
        {
            if (!repositoryImplementation.IsClass
                || repositoryImplementation.IsAbstract
                || repositoryImplementation.IsGenericTypeDefinition)
            {
                continue;
            }

            var repositoryInterfaces = repositoryImplementation.GetInterfaces()
                .Where(IsRepositoryContract)
                .ToArray();

            foreach (var repositoryInterface in repositoryInterfaces)
            {
                EnsureNotRegistered(serviceCollection, repositoryInterface, repositoryImplementation);
                serviceCollection.AddScoped(repositoryInterface, repositoryImplementation);
            }
        }

        return serviceCollection;
    }

    /// <summary>
    /// Определяет, является ли интерфейс поддерживаемым контрактом репозитория.
    /// </summary>
    /// <param name="interfaceType">Проверяемый тип интерфейса.</param>
    /// <returns><see langword="true"/>, если интерфейс нужно регистрировать; иначе <see langword="false"/>.</returns>
    private static bool IsRepositoryContract(Type interfaceType)
    {
        if (!interfaceType.IsInterface || interfaceType.IsGenericType)
        {
            return false;
        }

        return interfaceType.GetInterfaces().Any(parentInterface =>
        {
            return IsWriteRepositoryContract(parentInterface)
                || IsReadRepositoryContract(parentInterface);
        });
    }

    private static bool IsWriteRepositoryContract(Type interfaceType)
    {
        return interfaceType.IsGenericType
            && interfaceType.GetGenericTypeDefinition() == typeof(IRepository<,>);
    }

    private static bool IsReadRepositoryContract(Type interfaceType)
    {
        return interfaceType.IsGenericType
            && interfaceType.GetGenericTypeDefinition() == typeof(IReadRepository<>);
    }

    /// <summary>
    /// Проверяет, что контракт репозитория ещё не зарегистрирован в контейнере.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов.</param>
    /// <param name="repositoryInterface">Контракт репозитория.</param>
    /// <param name="repositoryImplementation">Реализация репозитория.</param>
    private static void EnsureNotRegistered(
        IServiceCollection serviceCollection,
        Type repositoryInterface,
        Type repositoryImplementation)
    {
        if (serviceCollection.Any(descriptor =>
        {
            return descriptor.ServiceType == repositoryInterface;
        }))
        {
            throw new InvalidOperationException(
                $"DAL интерфейс '{repositoryInterface.FullName}' уже зарегистрирован. " +
                $"Конфликт реализации: '{repositoryImplementation.FullName}'.");
        }
    }
}
