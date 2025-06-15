using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gradutionproject.Models;
using System.Reflection;

namespace Gradutionproject.Context
{
    public class graduationDbContext : IdentityDbContext<ApplicationUser>
    {
        internal object _userManager;

        public graduationDbContext(DbContextOptions<graduationDbContext> options)
            : base(options)
        {
        }

        // لو عندك جداول مخصصة
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Admin> Admins { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // لازم مع Identity

            // Course -> Lectures
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Lectures)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lecture -> Sections
            modelBuilder.Entity<Lecture>()
                .HasMany(l => l.Sections)
                .WithOne(s => s.Lecture)
                .HasForeignKey(s => s.LectureId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser -> Quizzes
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.User)
                .WithMany(u => u.Quizzes)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course -> Quizzes
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Email in Identity
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Admin -> Courses
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Courses)
                .WithOne(c => c.Admin)
                .HasForeignKey(c => c.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin -> Lectures
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Lectures)
                .WithOne(l => l.Admin)
                .HasForeignKey(l => l.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin -> Sections
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Sections)
                .WithOne(s => s.Admin)
                .HasForeignKey(s => s.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Seeding default Admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    Username = "Eslam",
                    Email = "Eslam@admin.com",
                    AdminPassword = "123456"
                }
            );

            // ✅ Seeding default Course (optional)
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Title = "Test Course",
                    ImageName = "default.jpg",
                    AdminId = 1
                }
            );
        }


    }
}