using Microsoft.EntityFrameworkCore;
using UpRentTestniZadatak.Model.Entities;
using User = UpRentTestniZadatak.Model.Entities.User;


public class AppDbContext : DbContext
{
    public DbSet<Role> Role { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserRole> UserRole { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.Visible);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.CreatedDate)
            .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<User>()
            .Property(u => u.ModifiedDate)
            .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Role>()
            .Property(u => u.ModifiedDate)
            .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Role>()
            .Property(u => u.CreatedDate)
            .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<UserRole>()
            .Property(u => u.ModifiedDate)
            .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<UserRole>()
            .Property(u => u.CreatedDate)
            .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

    }

}
