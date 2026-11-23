using System.Reflection;

namespace Organization.Product.ArchitectureTests.Definitions;

internal static class AssemblyExtensions
{
    internal static IEnumerable<Type> GetServiceTypes(this Assembly assembly)
    {
        return assembly.GetTypes().Where(type =>
            type.Namespace?.Split('.').Contains(ArchitectureDefinition.RepositoryExamplesDirectoryName) != true);
    }
}
