using Infrastructure.Ef.EfCore;
using Infrastructure.Ef.Core.Bootstrapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using PANiXiDA.Core.Ef.Migrator;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables();
    })
        .ConfigureServices((ctx, services) =>
        {
            services.AddPostgreSqlEfRepository<TemplateWriteDbContext, TemplateReadDbContext>(ctx.Configuration);
        })
    .RunMigrationsAsync<TemplateWriteDbContext>();