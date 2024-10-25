using FinalNet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace G6MVCDemo.DAL;

public class CMSDbContexts : IdentityDbContext<User, Role, int>
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=MOHAMED-ABOELLI;Database=G6CMS;Trusted_Connection=True;Encrypt=false");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Course>(course =>
        {
            course.HasKey("Id");
            course.HasOne<Instuctorclass>().WithMany().HasForeignKey(c => c.InstructorId);
        });
        modelBuilder.Entity<CourseEnrollment>(enr =>
        {
            enr.HasKey(e => new { e.CourseId, e.StudentId });

            enr.HasOne<Course>().WithMany(c => c.Enrollments).HasForeignKey(e => e.CourseId);
            enr.HasOne<Studentclass>().WithMany().HasForeignKey(e => e.StudentId);

        });

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = nameof(UserType.Student),
                NormalizedName = nameof(UserType.Student).ToUpper(),
            },
            new Role
            {
                Id = 2,
                Name = nameof(UserType.Instructor),
                NormalizedName = nameof(UserType.Instructor).ToUpper(),
            }
            );

    }
    public DbSet<Course> Courses { get; set; }

    public DbSet<Studentclass> Students { get; set; }
    public DbSet<Instuctorclass> Instuctors { get; set; }

    public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
}
