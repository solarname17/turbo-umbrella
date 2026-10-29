using Kadry.Models;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<Profession> Professions => Set<Profession>();
        public DbSet<Education> Educations => Set<Education>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Restrict запрещает удаление подразделения/должности/профессии/
            // образования, если на них ссылаются сотрудники (проверка также
            // дублируется в коде контроллеров с понятным сообщением).
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department).WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position).WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Profession).WithMany(p => p.Employees)
                .HasForeignKey(e => e.ProfessionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Education).WithMany(ed => ed.Employees)
                .HasForeignKey(e => e.EducationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasColumnType("decimal(18,2)");
        }
    }
}
