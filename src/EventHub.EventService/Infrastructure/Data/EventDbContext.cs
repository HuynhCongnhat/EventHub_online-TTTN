using EventHub.EventService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.EventService.Infrastructure.Data

{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
        {

        }

        public DbSet<Event> Events => Set<Event>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Location> Locations => Set<Location>();

        public DbSet<Schedule> Schedules => Set<Schedule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).IsRequired().HasMaxLength(200);

                entity.Property(x => x.Description).HasMaxLength(1000);

                entity.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).IsRequired().HasMaxLength(200);

                entity.Property(x => x.Address).IsRequired().HasMaxLength(500);

                entity.Property(x => x.City).HasMaxLength(100);

                entity.Property(x => x.Capacity).IsRequired();
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).IsRequired().HasMaxLength(250);

                entity.Property(x => x.Description).HasMaxLength(5000);

                entity.Property(x => x.ImageUrl).HasMaxLength(1000);

                entity.Property(x => x.Status).IsRequired().HasMaxLength(50);

                entity.HasOne(x => x.Category).WithMany(x => x.Events).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Location).WithMany(x => x.Events).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.Name);
                entity.HasIndex(x => x.CategoryId);
                entity.HasIndex(x => x.LocationId);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Note).HasMaxLength(1000);

                entity.HasOne(x => x.Event).WithMany(x => x.Schedules).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.EventId);
                entity.HasIndex(x => x.StartTime);
            });
        }
    }
}
