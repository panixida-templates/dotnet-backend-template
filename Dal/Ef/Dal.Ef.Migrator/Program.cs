using Dal.Ef;
using Dal.Ef.DependencyInjection;
using Dal.Ef.Migrator.Core;

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
        services.UseEfDal(ctx.Configuration);
    })
    .RunMigrationsAsync<DefaultDbContext>();
