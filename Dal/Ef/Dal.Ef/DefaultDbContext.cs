using Dal.Ef.DbModels;

using Microsoft.EntityFrameworkCore;

namespace Dal.Ef;

public class DefaultDbContext(DbContextOptions<DefaultDbContext> options) : DbContext(options)
{
    public virtual DbSet<SettingDbModel> Settings { get; set; }
    public virtual DbSet<UserDbModel> DomainUsers { get; set; }
}