using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Organization.Product.ArchitectureTests.Global;

using HttpServiceCollectionExtensions = PANiXiDA.Core.Presentation.Http.DependencyInjection.ServiceCollectionExtensions;

namespace Organization.Product.ArchitectureTests.Modules;

public sealed class ModuleConfigurationConventionTests
{
    private const string HostAssemblyName = "Organization.Product.Host";

    private const string ModuleConfigurationsNamespace =
        $"{HostAssemblyName}.Configurations.Modules";

    private const string SourceDirectoryName = "src";

    [Fact(DisplayName = "Modules should have host configurations in modules directory when discovered")]
    public void Modules_Should_HaveHostConfigurationsInModulesDirectory_When_Discovered()
    {
        var modules = ArchitectureDefinition.s_modules;
        var hostAssembly = ArchitectureDefinition.s_productionAssemblies.Single(
            assembly => string.Equals(
                assembly.GetName().Name,
                HostAssemblyName,
                StringComparison.Ordinal));
        var repositoryRoot = FindRepositoryRoot();

        var violations = modules
            .Select(module => GetConfigurationViolation(
                module: module,
                hostAssembly: hostAssembly,
                repositoryRoot: repositoryRoot))
            .OfType<string>()
            .ToArray();

        Assert.NotEmpty(modules);
        Assert.True(
            violations.Length == 0,
            $"Every module must have a host configuration in "
                + $"'{ModuleConfigurationsNamespace}':{Environment.NewLine}"
                + string.Join(Environment.NewLine, violations));
    }

    [Fact(DisplayName = "The common HTTP pipeline should be configured in the Host project")]
    public async Task HttpPipeline_Should_BeConfiguredInHost_When_Registered()
    {
        var httpExtensionsName = typeof(HttpServiceCollectionExtensions).FullName;
        var violations = await ProductionSourceDocumentDiscovery.GetItemsAsync<string>(
            async (_, document) =>
            {
                if (document.Project.AssemblyName == HostAssemblyName)
                {
                    return [];
                }

                var cancellationToken = TestContext.Current.CancellationToken;
                var root = await document.GetSyntaxRootAsync(cancellationToken);
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                if (root is null || semanticModel is null)
                {
                    return [];
                }

                return
                [
                    .. root.DescendantNodes()
                        .OfType<InvocationExpressionSyntax>()
                        .Where(invocation =>
                        {
                            var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol;
                            return symbol is IMethodSymbol { Name: "AddHttp" or "UseHttp" } method
                                && method.ContainingType.ToDisplayString() == httpExtensionsName;
                        })
                        .Select(invocation => $"{document.Name}: {invocation.Expression} must be called from Host.")
                ];
            });

        Assert.Empty(violations);
    }

    private static string? GetConfigurationViolation(
        ModuleArchitecture module,
        Assembly hostAssembly,
        string repositoryRoot)
    {
        var moduleName = module.Name[(module.Name.LastIndexOf('.') + 1)..];
        var configurationName = $"{moduleName}ModuleConfiguration";
        var configurationTypeName =
            $"{ModuleConfigurationsNamespace}.{configurationName}";
        var relativeConfigurationPath = Path.Combine(
            SourceDirectoryName,
            HostAssemblyName,
            "Configurations",
            "Modules",
            $"{configurationName}.cs");
        var configurationPath = Path.Combine(
            repositoryRoot,
            relativeConfigurationPath);

        if (hostAssembly.GetType(configurationTypeName) is null)
        {
            return $"Module '{module.Name}' must declare "
                + $"'{configurationTypeName}'.";
        }

        if (!File.Exists(configurationPath))
        {
            return $"Module '{module.Name}' configuration must reside at "
                + $"'{relativeConfigurationPath}'.";
        }

        return null;
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            if (Directory.Exists(Path.Combine(
                directory.FullName,
                SourceDirectoryName)))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not find repository root containing "
                + $"the '{SourceDirectoryName}' directory.");
    }
}
