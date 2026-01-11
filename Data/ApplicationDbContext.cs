using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure relationships and constraints if needed
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Brand)
            .WithMany(b => b.Vehicles)
            .HasForeignKey(v => v.BrandId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Customer)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
        .Property(c => c.IsActive)
        .HasDefaultValue(true);

        modelBuilder.Entity<Vehicle>()
        .Property(v => v.IsActive)
        .HasDefaultValue(true);

        modelBuilder.Entity<User>()
        .Property(u => u.IsActive)
        .HasDefaultValue(true);

        modelBuilder.Entity<UserPermission>()
        .HasOne(up => up.User)
        .WithMany(u => u.UserPermissions)
        .HasForeignKey(up => up.UserId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>()
        .HasOne(up => up.Permission)
        .WithMany(p => p.UserPermissions)
        .HasForeignKey(up => up.PermissionId)
        .OnDelete(DeleteBehavior.Restrict);

    }

}