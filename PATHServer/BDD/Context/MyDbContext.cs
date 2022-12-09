using Microsoft.EntityFrameworkCore;
using PATHServer.BDD.Models;
using System.Reflection;
using System.Reflection.Metadata;

public class MyDbContext : DbContext
{
    public DbSet<PATHUser> Users { get; set; }
    public DbSet<KeyConnexion> Keys { get; set; }
    public DbSet<ActionHistoryInfo> ActionHistoryInfos { get; set; }
    public DbSet<ActionHistory> ActionHistories { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<DataHistory> DataHistories { get; set; }
    public DbSet<DataHistoryDouble> DataHistoryDoubles { get; set; }
    public DbSet<DataHistoryDate> DataHistoryDates { get; set; }
    public DbSet<DataHistoryInt> DataHistoryInts { get; set; }
    public DbSet<DataHistoryString> DataHistoryStrings { get; set; }
    public DbSet<DataHistoryBool> DataHistoryBools { get; set; }

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
        base.OnModelCreating(modelBuilder);
    }
}