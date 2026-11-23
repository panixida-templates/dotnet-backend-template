using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Organization.Product.Module.Domain.RepositoryExamples.Users;
using Organization.Product.Module.Domain.RepositoryExamples.Users.Enumerations;
using Organization.Product.Module.Domain.RepositoryExamples.Users.ValueObjects;

namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Write;

internal sealed class UserConfiguration : AuditableEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasColumnOrder(0)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value).Value)
            .ValueGeneratedNever();

        builder.Property(item => item.Role)
            .HasConversion(
                role => role.Name,
                value => UserRole.Create(value).Value)
            .IsRequired();

        builder.Property(item => item.Name)
            .HasConversion(
                name => name.Value,
                value => UserName.Create(value).Value)
            .HasMaxLength(UserName.MaxLength)
            .IsRequired();

        builder.Property(item => item.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value).Value)
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.HasIndex(item => item.Email)
            .IsUnique();

        builder.Property(item => item.Phone)
            .HasConversion(
                phone => phone.Value,
                value => PhoneNumber.Create(value).Value)
            .HasMaxLength(PhoneNumber.MaxLength)
            .IsRequired();

        builder.HasIndex(item => item.Phone)
            .IsUnique();

        builder.Property(item => item.BirthDate)
            .HasConversion(
                birthDate => birthDate.Value,
                value => BirthDate.Create(value).Value)
            .IsRequired();

        builder.Property(item => item.Avatar)
            .HasConversion(
                avatar => avatar == null ? null : avatar.Value,
                value => Avatar.Create(value).Value)
            .HasMaxLength(Avatar.MaxLength)
            .IsRequired(false);
    }
}
