using Infrastructure.Ef.Core.Extensions;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Ef.Core.Contexts;

public abstract class ReadDbContext<TDbContext>(
    DbContextOptions<TDbContext> options) : DbContext(options)
    where TDbContext : ReadDbContext<TDbContext>
{
    protected virtual bool UseContextNameAsSchema { get; } = false;
    protected virtual bool ExcludeReadModelsFromMigrations { get; } = true;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var schemaName = UseContextNameAsSchema
            ? GetSchemaName()
            : null;

        modelBuilder.RegisterReadDbModels(
            typeof(TDbContext).Assembly,
            schemaName,
            ExcludeReadModelsFromMigrations);
    }

    private static string GetSchemaName()
    {
        return typeof(TDbContext).ToSchemaName(
            nameof(ReadDbContext<>),
            nameof(DbContext));
    }
}
