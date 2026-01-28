using Infrastructure.Persistence.Ef.DependencyInjection;
using Infrastructure.Persistence.Ef.EfCore;
using Infrastructure.Persistence.Ef.Migrator.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddJsonFile("appsettings.json")
       .AddEnvironmentVariables();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddEfDal(ctx.Configuration);
    })
    .RunMigrationsAsync<DefaultDbContext>();
