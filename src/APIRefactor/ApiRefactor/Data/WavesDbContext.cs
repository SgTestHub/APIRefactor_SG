using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ApiRefactor.Models;

namespace ApiRefactor.Data;

public class WavesDbContext : DbContext
{
    private const string ConnectionString = "Data Source=App_Data/waves.db";

    public DbSet<Wave> Waves { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Wave entity
        modelBuilder.Entity<Wave>()
            .HasKey(w => w.Id);

        modelBuilder.Entity<Wave>()
            .Property(w => w.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Wave>()
            .Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Wave>()
            .Property(w => w.WaveDate)
            .IsRequired();
    }

    public SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        return connection;
    }

    public void EnsureConnectionOpen(SqliteConnection connection)
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public void CloseConnection(SqliteConnection connection)
    {
        if (connection != null && connection.State != System.Data.ConnectionState.Closed)
        {
            connection.Close();
        }
    }
}
