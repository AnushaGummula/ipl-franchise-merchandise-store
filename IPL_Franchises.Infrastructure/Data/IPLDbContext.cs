using IPL_Franchises.Domain.Entities;
using IPL_Franchises.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Infrastructure.Data;

public class IPLDbContext
    : IdentityDbContext<ApplicationUser>
{
    public IPLDbContext(
        DbContextOptions<IPLDbContext> options)
        : base(options)
    {
    }

    public DbSet<Franchise> Franchises =>
        Set<Franchise>();

    public DbSet<Product> Products =>
        Set<Product>();

    public DbSet<Cart> Carts =>
        Set<Cart>();

    public DbSet<CartItem> CartItems =>
        Set<CartItem>();

    public DbSet<Order> Orders =>
        Set<Order>();

    public DbSet<OrderItem> OrderItems =>
        Set<OrderItem>();


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        /*
         * FRANCHISE
         */

        modelBuilder.Entity<Franchise>()
            .HasIndex(f => f.Code)
            .IsUnique();


        /*
         * PRODUCT
         */

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);


        /*
         * CART
         */

        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.UserId)
            .IsUnique();


        /*
         * CART ITEM
         */

        modelBuilder.Entity<CartItem>()
     .HasIndex(ci => new
     {
         ci.CartId,
         ci.ProductId,
         ci.SelectedSize
     })
     .IsUnique();

        modelBuilder.Entity<CartItem>()
            .Property(ci => ci.SelectedSize)
            .HasMaxLength(5);

        /*
         * ORDER
         */

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();


        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);


        modelBuilder.Entity<OrderItem>()
    .Property(x => x.FranchiseCode)
    .HasMaxLength(20);

        modelBuilder.Entity<OrderItem>()
            .Property(x => x.ProductType)
            .HasMaxLength(50);

        modelBuilder.Entity<OrderItem>()
            .Property(x => x.SelectedSize)
            .HasMaxLength(5);

        /*
         * ORDER ITEM
         */

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);


        /*
         * IDENTITY USER
         */

        modelBuilder
            .Entity<ApplicationUser>()
            .Property(user => user.FullName)
            .HasMaxLength(150);
    }
}
