using Microsoft.CodeAnalysis.MSBuild;

using Organization.Product.ArchitectureTests.Global;

[assembly: AssemblyFixture(typeof(ProductionSourceWorkspaceFixture))]

namespace Organization.Product.ArchitectureTests.Global;

public sealed class ProductionSourceWorkspaceFixture : IDisposable
{
    internal static MSBuildWorkspace Workspace { get; } = MSBuildWorkspace.Create();

    public void Dispose()
    {
        Workspace.Dispose();
    }
}
