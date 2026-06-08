using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ApiRefactor.Models;

namespace ApiRefactor.Data;

public class WavesDbContext : DbContext
{
    private const string ConnectionString = "Data Source=App_Data/waves.db";

    public DbSet<Wave> Waves { get; set; }
    public DbSet<LocalUser> LocalUsers { get; set; }

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
        modelBuilder.Entity<Wave>(w =>
        {
            w.HasKey("Id");

            w.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            w.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);

            w.Property(w => w.WaveDate)
                .IsRequired();
        });

        modelBuilder.Entity<LocalUser>(b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer");

            b.Property<string>("Email")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Password")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Role")
                .IsRequired()
                .HasColumnType("text");

            b.HasKey("Id");

            b.ToTable("LocalUsers");
        });
        // Seed demo users for testing authentication
        modelBuilder.Entity<LocalUser>().HasData(
            new LocalUser
            {
                Id = 1,
                Name = "Admin User",
                Email = "admin@gmail.com",
                Password = "admin123", // In production, hash passwords!
                Role = "Admin"
            },
            new LocalUser
            {
                Id = 2,
                Name = "Test User",
                Email = "test@gmail.com",
                Password = "test123",
                Role = "User"
            }
        );
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
