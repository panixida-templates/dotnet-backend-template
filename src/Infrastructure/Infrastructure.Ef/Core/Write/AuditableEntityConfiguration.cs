using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Ef.Core.Write;

/// <summary>
/// Содержит константы, используемые в EF инфраструктуре.
/// </summary>
public static class EfConstants
{
    /// <summary>
    /// Имя строки подключения к PostgreSQL в конфигурации приложения.
    /// </summary>
    public const string PostgreSqlConnectionStringName = "PostgreSqlConnectionString";

    /// <summary>
    /// Имя shadow-свойства, содержащего дату и время создания сущности.
    /// </summary>
    public const string CreatedAt = "CreatedAt";

    /// <summary>
    /// Имя shadow-свойства, содержащего дату и время последнего изменения сущности.
    /// </summary>
    public const string UpdatedAt = "UpdatedAt";

    /// <summary>
    /// Имя shadow-свойства, содержащего дату и время мягкого удаления сущности.
    /// </summary>
    public const string DeletedAt = "DeletedAt";
}

/// <summary>
/// Предоставляет базовую конфигурацию аудируемой сущности со shadow-свойствами аудита и поддержкой soft-delete.
/// </summary>
public abstract class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Применяет конфигурацию сущности, shadow-свойства аудита и фильтр soft-delete.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureEntity(builder);
        ConfigureAudit(builder);
        ConfigureSoftDelete(builder);
    }

    /// <summary>
    /// Настраивает конфигурацию, специфичную для конкретной сущности.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);

    /// <summary>
    /// Добавляет shadow-свойства, используемые для аудита сущности.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    protected virtual void ConfigureAudit(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property<DateTime>(EfConstants.CreatedAt)
            .IsRequired()
            .HasColumnOrder(1);

        builder.Property<DateTime>(EfConstants.UpdatedAt)
            .IsRequired()
            .HasColumnOrder(2);

        builder.Property<DateTime?>(EfConstants.DeletedAt)
            .HasColumnOrder(3);
    }

    /// <summary>
    /// Применяет глобальный фильтр, скрывающий записи, помеченные как удаленные.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    protected virtual void ConfigureSoftDelete(EntityTypeBuilder<TEntity> builder)
    {
        if (!IsSoftDeleteEnabled())
        {
            return;
        }

        builder.HasQueryFilter(item =>
            EF.Property<DateTime?>(item, EfConstants.DeletedAt) == null);
    }

    /// <summary>
    /// Определяет, должен ли для сущности использоваться soft-delete.
    /// </summary>
    /// <returns>
    /// <see langword="true"/>, если глобальный фильтр soft-delete должен быть включен; иначе <see langword="false"/>.
    /// </returns>
    protected virtual bool IsSoftDeleteEnabled()
    {
        return true;
    }
}
