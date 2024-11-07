using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PPE.Core;

namespace PPE.DataModel;

public class CommonDbContext : IdentityDbContext<Base_User, Base_Role, string, Base_UserClaim, Base_UserRole, Base_UserLogin, Base_RoleClaim, Base_UserToken>
{
    public CommonDbContext()
    {
    }

    public CommonDbContext(DbContextOptions<CommonDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Base_User>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_User)));
            b.HasMany<Base_UserMenu>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
            b.HasMany<Base_UserButton>().WithOne().HasForeignKey(u => u.UserId).IsRequired();
            b.Property(u => u.UserName).HasMaxLength(128);
            b.Property(u => u.NormalizedUserName).HasMaxLength(128);
            b.Property(u => u.Email).HasMaxLength(128);
            b.Property(u => u.NormalizedEmail).HasMaxLength(128);
        });
        builder.Entity<Base_UserClaim>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_UserClaim)));
        });
        builder.Entity<Base_UserLogin>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_UserLogin)));
            b.Property(l => l.ProviderKey).HasMaxLength(128);
            b.Property(l => l.LoginProvider).HasMaxLength(128);
        });
        builder.Entity<Base_UserToken>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_UserToken)));
            b.Property(t => t.LoginProvider).HasMaxLength(100);
            b.Property(t => t.Name).HasMaxLength(100);
        });
        builder.Entity<Base_Role>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_Role)));
            b.HasMany<Base_RoleMenu>().WithOne().HasForeignKey(r => r.RoleId).IsRequired();
            b.HasMany<Base_RoleButton>().WithOne().HasForeignKey(r => r.RoleId).IsRequired();
            b.Property(r => r.Name).HasMaxLength(128);
            b.Property(r => r.NormalizedName).HasMaxLength(128);
        });
        builder.Entity<Base_UserRole>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_UserRole)));
        });
        builder.Entity<Base_RoleClaim>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_RoleClaim)));
        });
        builder.Entity<Base_OperationLog>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_OperationLog)));
            b.HasMany<Base_OperationLogDetail>().WithOne().HasForeignKey(o => o.OperateLogId).IsRequired();
        });
        builder.Entity<Base_OperationLogDetail>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_OperationLogDetail)));
        });
        builder.Entity<Base_Menu>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_Menu)));
            b.HasIndex(m => m.NormalizedName).IsUnique();
            b.Property(m => m.ConcurrencyStamp).IsConcurrencyToken();
            b.HasMany<Base_Button>().WithOne().HasForeignKey(m => m.MenuId).IsRequired();
            b.HasMany<Base_RoleMenu>().WithOne().HasForeignKey(m => m.MenuId).IsRequired();
            b.HasMany<Base_UserMenu>().WithOne().HasForeignKey(m => m.MenuId).IsRequired();
        });

        builder.Entity<Base_Button>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_Button)));
            b.Property(b => b.ConcurrencyStamp).IsConcurrencyToken();
            b.HasMany<Base_RoleButton>().WithOne().HasForeignKey(b => b.ButtonId).IsRequired();
            b.HasMany<Base_UserButton>().WithOne().HasForeignKey(b => b.ButtonId).IsRequired();
        });
        builder.Entity<Base_RoleMenu>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_RoleMenu)));
            b.HasKey(rm => new { rm.RoleId, rm.MenuId });
        });
        builder.Entity<Base_RoleButton>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_RoleButton)));
            b.HasKey(rb => new { rb.RoleId, rb.ButtonId });
        });
        builder.Entity<Base_UserMenu>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_UserMenu)));
            b.HasKey(um => new { um.UserId, um.MenuId });
        });
        builder.Entity<Base_UserButton>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_UserButton)));
            b.HasKey(ub => new { ub.UserId, ub.ButtonId });
        });

        builder.Entity<Base_SystemConfig>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_SystemConfig)));
            b.HasIndex(s => s.NormalizedName).IsUnique();
            b.Property(s => s.ConcurrencyStamp).IsConcurrencyToken();
        });


    }

    public DbSet<Base_OperationLog> OperationLogs { get; set; }
    public DbSet<Base_OperationLogDetail> OperationLogDetails { get; set; }
    public DbSet<Base_SignLog> SignLogs { get; set; }
    public DbSet<Base_Menu> Menus { get; set; }
    public DbSet<Base_Button> Buttons { get; set; }
    public DbSet<Base_RoleMenu> RoleMenus { get; set; }
    public DbSet<Base_RoleButton> RoleButtons { get; set; }
    public DbSet<Base_UserMenu> UserMenus { get; set; }
    public DbSet<Base_UserButton> UserButtons { get; set; }
    public DbSet<Base_SystemConfig> SystemConfigs { get; set; }

}