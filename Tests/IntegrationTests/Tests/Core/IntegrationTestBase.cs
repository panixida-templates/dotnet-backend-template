using Common.Clients.Interfaces;
using Common.Constants.ApiEndpoints.Core;
using Common.SearchParams.Core;

using DataGenerator;

using IntegrationTests.Constants;
using IntegrationTests.DataFactories;
using IntegrationTests.Infrastructure.Core;
using IntegrationTests.WebApplicationFactories;

using Microsoft.Extensions.DependencyInjection;

using Pl.Api.Http.Dtos.Models.Core;

using Xunit;

using static IntegrationTests.Constants.TraitsConstants;

namespace IntegrationTests.Tests.Core;

[Collection(CollectionConstants.IntegrationCollection)]
[Trait(TraitKeysConstants.Category, TraitCategoriesConstants.Integration)]
public abstract partial class IntegrationTestBase<TEndpoint, TId, TModel, TSearchParams, TConvertParams>
    where TEndpoint : IBaseApiEndpointsConstants<TEndpoint, TId>
    where TId : struct
    where TModel : BaseDto<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class
{
    protected readonly IDictionary<string, string?> DefaultHeaders;
    protected readonly IApiHttpClient ApiHttpClient;

    private readonly TestContainers _testContainers;

    protected DataFacade TestDataFacade;

    protected IntegrationTestBase(ApiWebApplicationFactory apiWebApplicationFactory)
    {
        DefaultHeaders = TestHeadersFactory.CreateAuthHeadersForUser();
        ApiHttpClient = apiWebApplicationFactory.Services.GetRequiredService<IApiHttpClient>();

        _testContainers = apiWebApplicationFactory.Services.GetRequiredService<TestContainers>();

        TestDataFacade = new DataFacade(scope: GetType().FullName);
    }

    protected async Task ResetDatabaseAsync()
    {
        await _testContainers.ResetDatabaseAsync();
    }

    protected async Task<TId> CreateNotFoundIdAsync()
    {
        var id = await CreateAsync();
        await DeleteAsync(id);
        return id;
    }
}
