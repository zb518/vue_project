using Microsoft.EntityFrameworkCore;
using PPE.Core;
using PPE.DataModel;

namespace PPE.WebCore;

public class CompleteDbContext : CommonDbContext
{
    public CompleteDbContext(DbContextOptions<CommonDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Base_Vocabulary>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_Vocabulary)));
            b.Property(w => w.ConcurrencyStamp).IsConcurrencyToken();
        });

        builder.Entity<Base_Course>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_Course)));
            b.HasIndex(c => c.NormalizedCode).IsUnique();
            b.Property(c => c.ConcurrencyStamp).IsConcurrencyToken();
            b.HasMany<Base_CourseCatalogue>().WithOne().HasForeignKey(c => c.CourseId).IsRequired();
            b.HasMany<Base_CourseMajor>().WithOne().HasForeignKey(c => c.CourseId).IsRequired();
        });

        builder.Entity<Base_Major>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_Major)));
            b.HasIndex(c => c.NormalizedCode).IsUnique();
            b.HasIndex(c => c.NormalizedName).IsUnique();
            b.Property(c => c.ConcurrencyStamp).IsConcurrencyToken();
            b.HasMany<Base_CourseMajor>().WithOne().HasForeignKey(c => c.MajorId).IsRequired();
        });

        builder.Entity<Base_CourseMajor>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_CourseMajor)));
            b.HasKey(mc => new { mc.CourseId, mc.MajorId });
        });

        builder.Entity<Base_CourseCatalogue>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_CourseCatalogue)));
            b.Property(c => c.ConcurrencyStamp).IsConcurrencyToken();
        });

        builder.Entity<Base_CourseContent>(b =>
        {
            b.ToTable(EntityHelper.GetTableName(typeof(Base_CourseContent)));
            //b.HasIndex(mc => new { mc.CourseId, mc.MajorId });
        });
    }

    public DbSet<Base_Vocabulary> Vocabularies { get; set; }
    public DbSet<Base_Course> Courses { get; set; }
    public DbSet<Base_Major> Majors { get; set; }
    public DbSet<Base_CourseMajor> CourseMajors { get; set; }
    public DbSet<Base_CourseCatalogue> CourseCatalogues { get; set; }
    public DbSet<Base_CourseContent> CourseContents { get; set; }
}
