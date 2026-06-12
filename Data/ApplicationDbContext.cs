using FoodDeliverySystem.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FoodDeliverySystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        // ================= DB SETS =================
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =====================================================
            // APPLICATION USER (Identity)
            // =====================================================
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName)
                    .HasMaxLength(150);
            });

            // =====================================================
            // RESTAURANT
            // =====================================================
            builder.Entity<Restaurant>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(r => r.Description)
                    .HasMaxLength(500);

                entity.Property(r => r.Address)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(r => r.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(r => r.IsActive)
                    .HasDefaultValue(true);

                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationship: User (Owner) → Restaurants
                entity.HasOne(r => r.Owner)
                    .WithMany(u => u.OwnedRestaurants)
                    .HasForeignKey(r => r.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => r.OwnerId);
            });

            // =====================================================
            // FOOD ITEM
            // =====================================================
            builder.Entity<FoodItem>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(f => f.Description)
                    .HasMaxLength(500);

                entity.Property(f => f.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(f => f.IsAvailable)
                    .HasDefaultValue(true);

                entity.Property(f => f.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationship: Restaurant → FoodItems
                entity.HasOne(f => f.Restaurant)
                    .WithMany(r => r.FoodItems)
                    .HasForeignKey(f => f.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(f => f.RestaurantId);
            });

            // =====================================================
            // ORDER
            // =====================================================
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(o => o.DeliveryAddress)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(o => o.Status)
                    .HasConversion<int>();

                entity.Property(o => o.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Customer → Orders
                entity.HasOne(o => o.Customer)
                    .WithMany(u => u.CustomerOrders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Restaurant → Orders
                entity.HasOne(o => o.Restaurant)
                    .WithMany(r => r.Orders)
                    .HasForeignKey(o => o.RestaurantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.CustomerId);
                entity.HasIndex(o => o.RestaurantId);
            });

            // =====================================================
            // ORDER ITEM
            // =====================================================
            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(oi => oi.Quantity)
                    .IsRequired();

                // Order → OrderItems
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FoodItem → OrderItems
                entity.HasOne(oi => oi.FoodItem)
                    .WithMany()
                    .HasForeignKey(oi => oi.FoodItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(oi => oi.OrderId);
            });

            // =====================================================
            // DELIVERY (NO DeliveryBoy ENTITY)
            // =====================================================
            builder.Entity<Delivery>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.Status)
                    .HasConversion<int>();

                entity.Property(d => d.AssignedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Order → Delivery (ONE TO ONE)
                entity.HasOne(d => d.Order)
                    .WithOne(o => o.Delivery)
                    .HasForeignKey<Delivery>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

               

                entity.HasIndex(d => d.OrderId).IsUnique();
                entity.HasIndex(d => d.DeliveryBoyId);
            });
        }
    }
}
