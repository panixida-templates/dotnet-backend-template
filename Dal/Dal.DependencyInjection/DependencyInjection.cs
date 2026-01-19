using Common.Constants;

using Dal.Ef.DependencyInjection;

using Dal.MongoDb.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dal.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseDal(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>(AppsettingsKeysConstants.DalUseEf))
        {
            serviceCollection.UseEfDal(configuration);
        }
        if (configuration.GetValue<bool>(AppsettingsKeysConstants.DalUseMongo))
        {
            serviceCollection.UseMongoDal(configuration);
        }

        return serviceCollection;
    }
}