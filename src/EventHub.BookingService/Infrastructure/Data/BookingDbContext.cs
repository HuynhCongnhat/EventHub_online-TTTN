using EventHub.BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.BookingService.Infrastructure.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext (DbContextOptions<BookingDbContext> options) : base(options)
        {
        }

        public DbSet<TicketInventory> TicketInventories => Set<TicketInventory>();

        public DbSet<BookingOrder> BookingOrders => Set<BookingOrder>();

        public DbSet<BookingItem> BookingItems => Set<BookingItem>();

        public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TicketInventory>(entity =>
            {
                entity.ToTable("TicketInventories");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
                entity.Property(x => x.Description).HasMaxLength(500);
                entity.Property(x => x.Price).HasPrecision(18, 2);
                entity.Property(x => x.TotalQuantity).IsRequired();
                entity.Property(x => x.SoldQuantity).IsRequired();
                entity.Property(x => x.HeldQuantity).IsRequired();
                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.CreatedAt).IsRequired();
                entity.HasIndex(x => new { x.EventId, x.Name }).IsUnique();
            });

            modelBuilder.Entity<BookingOrder>(entity =>
            {
                entity.ToTable("BookingOrders");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.OrderCode).IsRequired().HasMaxLength(50);
                entity.HasIndex(x => x.OrderCode).IsUnique();
                entity.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.ExpiresAt).IsRequired();
                entity.Property(x => x.CreatedAt).IsRequired();
                entity.Property(x => x.UpdatedAt).IsRequired();
                entity.HasMany(x => x.Items).WithOne(x => x.BookingOrder).HasForeignKey(x => x.BookingOrderId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookingItem>(entity =>
            {
                entity.ToTable("BookingItems");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Quantity).IsRequired();
                entity.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
                entity.Property(x => x.TotalPrice).HasPrecision(18, 2).IsRequired();
                entity.Property(x => x.CreatedAt).IsRequired();
                entity.HasOne(x => x.TicketInventory).WithMany().HasForeignKey(x => x.TicketInventoryId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.ToTable("BookingSeats");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.EventId).IsRequired();
                entity.Property(x => x.SeatCode).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.HeldUntil);
                entity.Property(x => x.CreatedAt).IsRequired();
                entity.HasOne(x => x.TicketInventory).WithMany().HasForeignKey(x => x.TicketInventoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.BookingOrder).WithMany().HasForeignKey(x => x.BookingOrderId).OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(x => new { x.EventId, x.SeatCode }).IsUnique();
            });
        }


    }
}
