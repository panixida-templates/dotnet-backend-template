using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            .HasConversion(s_userIdConverter)
            .ValueGeneratedNever();

        builder.Property(item => item.Role)
            .HasConversion(s_userRoleConverter)
            .IsRequired();

        builder.Property(item => item.Name)
            .HasConversion(s_userNameConverter)
            .HasMaxLength(UserName.MaxLength)
            .IsRequired();

        builder.Property(item => item.Email)
            .HasConversion(s_emailConverter)
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.HasIndex(item => item.Email)
            .IsUnique();

        builder.Property(item => item.Phone)
            .HasConversion(s_phoneNumberConverter)
            .HasMaxLength(PhoneNumber.MaxLength)
            .IsRequired();

        builder.HasIndex(item => item.Phone)
            .IsUnique();

        builder.Property(item => item.BirthDate)
            .HasConversion(s_birthDateConverter)
            .IsRequired();

        builder.Property(item => item.Avatar)
            .HasConversion(s_avatarConverter)
            .HasMaxLength(Avatar.MaxLength)
            .IsRequired(false);
    }

    private static readonly ValueConverter<UserId, Guid> s_userIdConverter = new(
        userId => userId.Value,
        value => UserId.Create(value).Value);

    private static readonly ValueConverter<UserRole, string> s_userRoleConverter = new(
        role => role.Name,
        value => UserRole.Create(value).Value);

    private static readonly ValueConverter<UserName, string> s_userNameConverter = new(
        name => name.Value,
        value => UserName.Create(value).Value);

    private static readonly ValueConverter<Email, string> s_emailConverter = new(
        email => email.Value,
        value => Email.Create(value).Value);

    private static readonly ValueConverter<PhoneNumber, string> s_phoneNumberConverter = new(
        phone => phone.Value,
        value => PhoneNumber.Create(value).Value);

    private static readonly ValueConverter<BirthDate, DateOnly> s_birthDateConverter = new(
        birthDate => birthDate.Value,
        value => BirthDate.Create(value).Value);

    private static readonly ValueConverter<Avatar?, string?> s_avatarConverter = new(
        avatar => avatar == null ? null : avatar.Value,
        value => Avatar.Create(value).Value);
}
