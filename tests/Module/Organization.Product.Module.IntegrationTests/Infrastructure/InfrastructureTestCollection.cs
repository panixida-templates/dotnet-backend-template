namespace Organization.Product.Module.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class InfrastructureTestCollection : ICollectionFixture<InfrastructureTestFixture>
{
    public const string Name = "Infrastructure";
}
