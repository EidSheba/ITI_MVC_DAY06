using Microsoft.EntityFrameworkCore;
using CourseManagement.Data.Models;

namespace CourseManagement.Data.Context
{
    /// <summary>
    /// Database context for Entity Framework Core
    /// سياق قاعدة البيانات لـ Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships / تكوين العلاقات
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes / تكوين الفهارس
            modelBuilder.Entity<Course>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Seed initial data / بيانات أولية
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // ثابت GUID (ما يتغيرش)
            var instructorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    Id = instructorId,
                    FirstName = "John",
                    LastName = "Doe",
                    Bio = "Experienced software developer and instructor",
                    Specialization = Specialization.SoftwareDevelopment,
                    IsActive = true
                }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Introduction to C#",
                    Description = "Learn the fundamentals of C# programming",
                    Category = Category.Programming,
                    StartDate = new DateTime(2025, 01, 01),
                    EndDate = new DateTime(2025, 01, 31),
                    IsActive = true,
                    InstructorId = instructorId
                }
            );
        }
    }
}
