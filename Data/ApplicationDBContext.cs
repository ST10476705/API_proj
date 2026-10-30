using API_proj.Models;
using Microsoft.EntityFrameworkCore;

namespace API_proj.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Assignment> Assignments => Set<Assignment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique constraint from DB: UQ_ASSIGNMENT_UserRole
            modelBuilder.Entity<Assignment>()
                .HasIndex(a => new { a.UserId, a.RoleId })
                .IsUnique();

            // Unique constraint: UQ_ENROLMENTS_UserCategory
            modelBuilder.Entity<Enrolment>()
                .HasIndex(e => new { e.UserId, e.CategoryId })
                .IsUnique();

            // Unique constraint: UQ_RESULTS_Enrolment
            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EnrolmentId)
                .IsUnique();

            // ✅ Assignment → User (one User, many Assignments)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Assignment → Role (one Role, many Assignments)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Assignments)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Enrolment → Category
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Result → Enrolment (1:1)
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithMany()
                .HasForeignKey(r => r.EnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}