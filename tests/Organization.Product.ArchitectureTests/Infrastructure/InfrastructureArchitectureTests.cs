using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Organization.Product.ArchitectureTests.Infrastructure;

public sealed class InfrastructureArchitectureTests
{
    [Fact(DisplayName = "Infrastructure repositories should be internal")]
    public void InfrastructureRepositories_Should_Be_Internal()
    {
        Classes().That()
            .ResideInAssemblyMatching(ArchitectureDefinition.InfrastructureAssemblyNamePattern)
            .And().ResideInNamespaceMatching(ArchitectureDefinition.IncludeNamespaceSegment("Persistence.Features"))
            .And().HaveNameEndingWith("Repository")
            .Should().BeInternal()
            .Check(ArchitectureDefinition.Architecture);
    }

    [Fact(DisplayName = "Infrastructure read database models should be internal")]
    public void InfrastructureReadDatabaseModels_Should_Be_Internal()
    {
        Classes().That()
            .ResideInAssemblyMatching(ArchitectureDefinition.InfrastructureAssemblyNamePattern)
            .And().ResideInNamespaceMatching(ArchitectureDefinition.IncludeNamespaceSegment("Persistence.Features"))
            .And().HaveNameEndingWith("ReadDbModel")
            .Should().BeInternal()
            .Check(ArchitectureDefinition.Architecture);
    }

    [Fact(DisplayName = "Infrastructure mappers should be internal")]
    public void InfrastructureMappers_Should_Be_Internal()
    {
        Classes().That()
            .ResideInAssemblyMatching(ArchitectureDefinition.InfrastructureAssemblyNamePattern)
            .And().ResideInNamespaceMatching(ArchitectureDefinition.IncludeNamespaceSegment("Persistence.Features"))
            .And().HaveNameEndingWith("Mapper")
            .Should().BeInternal()
            .Check(ArchitectureDefinition.Architecture);
    }
}
