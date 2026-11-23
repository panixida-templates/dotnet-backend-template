namespace Organization.Product.Module.FunctionalTests.Presentation;

[CollectionDefinition(Name)]
public sealed class FunctionalTestCollectionDefinition : ICollectionFixture<FunctionalTestFixture>
{
    public const string Name = "Functional";
}
