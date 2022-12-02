using Microsoft.EntityFrameworkCore;
using PATHServer.BDD.Models;
using System.Reflection;
using System.Reflection.Metadata;

public class MyDbContext : DbContext
{
    public DbSet<PATHUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=TestDatabase.db", options =>
        {
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map table names
        modelBuilder.Entity<SensorData>().ToTable("Blogs", "test");
        modelBuilder.Entity<SensorData>(entity =>
        {
            entity.HasKey(e => e.Data);
            entity.HasIndex(e => e.SensorId).IsUnique();
            entity.Property(e => e.DateTimeAdd).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        base.OnModelCreating(modelBuilder);
    }
}