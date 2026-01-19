using Dal.Ef.DbModels.Core;

using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Npgsql.NameTranslation;

using System.Reflection;

namespace Dal.Ef;

public sealed class DefaultDbContext(DbContextOptions<DefaultDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        RegisterDbModels(modelBuilder, typeof(BaseDbModel<>).Assembly);
        ConfigureTableNames(modelBuilder);
        ConfigureGuidKeyGeneration(modelBuilder);
    }

    private static void RegisterDbModels(ModelBuilder modelBuilder, Assembly assembly)
    {
        var entityTypes = assembly.GetTypes().Where(type => type is { IsClass: true, IsAbstract: false } && IsBaseDbModel(type));

        foreach (var entityType in entityTypes)
        {
            modelBuilder.Entity(entityType);
        }
    }

    private static void ConfigureTableNames(ModelBuilder modelBuilder)
    {
        var translator = new NpgsqlSnakeCaseNameTranslator();

        foreach (var clrType in modelBuilder.Model.GetEntityTypes().Select(item => item.ClrType))
        {
            if (!IsBaseDbModel(clrType))
            {
                continue;
            }

            var name = clrType.Name;
            name = TrimSuffix(name, "DbModel");
            name = name.Pluralize();
            name = translator.TranslateTypeName(name);

            modelBuilder.Entity(clrType).ToTable(name);
        }
    }

    private static string TrimSuffix(string name, string suffix)
    {
        if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            return name[..^suffix.Length];
        }

        return name;
    }

    private static void ConfigureGuidKeyGeneration(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (!IsBaseDbModel(clrType, typeof(Guid)))
            {
                continue;
            }

            var idProperty = entityType.FindProperty(nameof(BaseDbModel<>.Id));
            if (idProperty == null)
            {
                continue;
            }

            idProperty.ValueGenerated = ValueGenerated.OnAdd;
            idProperty.SetDefaultValueSql("gen_random_uuid()");
        }
    }

    private static bool IsBaseDbModel(Type type, Type? idType = null)
    {
        return IsDerivedFromGenericBase(type, typeof(BaseDbModel<>), idType);
    }

    private static bool IsDerivedFromGenericBase(Type type, Type genericBaseType, Type? genericArgument = null)
    {
        var current = type;

        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == genericBaseType)
            {
                var args = current.GetGenericArguments();
                if (genericArgument == null || args.Length > 0 && args[0] == genericArgument)
                {
                    return true;
                }
            }

            current = current.BaseType;
        }

        return false;
    }
}
