using System.Linq.Expressions;

using Infrastructure.Ef.Core.Read;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Ef.Core.Extensions;

internal static class AuditableReadDbModelExtensions
{
    public static ModelBuilder ConfigureAuditableReadDbModels(this ModelBuilder modelBuilder)
    {
        var clrTypes = modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType)
            .Where(IsAuditableReadDbModel);

        foreach (var clrType in clrTypes)
        {
            var builder = modelBuilder.Entity(clrType);
            builder.HasQueryFilter(CreateSoftDeleteFilter(clrType));
        }

        return modelBuilder;
    }

    private static bool IsAuditableReadDbModel(Type type)
    {
        var currentType = type;

        while (currentType is not null && currentType != typeof(object))
        {
            if (currentType.IsGenericType
                && currentType.GetGenericTypeDefinition() == typeof(AuditableReadDbModel<>))
            {
                return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }

    private static LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "item");

        var deletedAtProperty = Expression.Property(
            parameter,
            nameof(AuditableReadDbModel<>.DeletedAt));

        var body = Expression.Equal(
            deletedAtProperty,
            Expression.Constant(null, typeof(DateTime?)));

        return Expression.Lambda(body, parameter);
    }
}
