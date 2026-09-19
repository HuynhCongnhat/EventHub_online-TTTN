using EventHub.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHub.AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

        public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships and constraints
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(r => r.Name)
                    .IsUnique();

                entity.HasData(
                    new Role
                    {
                        Id = 1,
                        Name = "Admin"
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "Customer"
                    }
                );
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasIndex(r => r.Token)
                    .IsUnique();

                entity.HasOne(r => r.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.TokenHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasIndex(r => r.TokenHash)
                    .IsUnique();

                entity.Property(r => r.ExpiresAt)
                    .IsRequired();

                entity.Property(r => r.IsUsed)
                    .IsRequired();

                entity.Property(r => r.CreatedAt)
                    .IsRequired();

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EmailVerificationToken>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.ExpiresAt)
                    .IsRequired();

                entity.Property(e => e.IsUsed)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
