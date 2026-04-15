using EasySchedule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Profession> Professions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Profession>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Surname).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Profession)
                  .WithMany(p => p.Employees)
                  .HasForeignKey(e => e.ProfessionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}