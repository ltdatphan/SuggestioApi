using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SuggestioApi.Models;

namespace SuggestioApi.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        // public DbSet<User> Users { get; set; } = null!;
        public DbSet<CuratedList> CuratedLists { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Follow> Follows { get; set; } = null!;
        // public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                        // .UseNpgsql(IConfiguration.GetConnectionString("SuggestioApiDatabase"))
                        .UseValidationCheckConstraints();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Config default roles
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole {
                    Name="Admin",
                    NormalizedName="ADMIN",
                },
                new IdentityRole {
                    Name="User",
                    NormalizedName="USER",
                },
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            //Identity Constraints
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(login => new { login.LoginProvider, login.ProviderKey });

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(role => new { role.UserId, role.RoleId });

            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(token => new { token.UserId, token.LoginProvider, token.Name });

            // User Constraints

            // List Constraints
            modelBuilder.Entity<CuratedList>(entity =>
            {
                entity
                    .HasIndex(cl => cl.OwnerId)
                    .HasDatabaseName("IX_List_OwnerId");

                entity
                    .Property(l => l.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity
                    .Property(l => l.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity
                    .HasOne(l => l.User)
                    .WithMany(u => u.CuratedLists)
                    .HasForeignKey(l => l.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //Item Constraints
            modelBuilder.Entity<Item>(entity =>
            {
                entity
                    .HasIndex(i => i.ListId)
                    .HasDatabaseName("IX_Item_ListId");

                entity
                    .HasOne(i => i.CuratedList)
                    .WithMany(l => l.Items)
                    .HasForeignKey(i => i.ListId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .Property(l => l.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity
                    .Property(l => l.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            });

            //Follow Constraints
            modelBuilder.Entity<Follow>(entity =>
            {
                entity
                    .HasKey(f => f.Id);

                entity
                    .HasIndex(f => new { f.CurrentUserId, f.TargetUserId })
                    .IsUnique();

                entity
                    .Property(l => l.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity
                    .HasOne(f => f.TargetUser)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.TargetUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(f => f.CurrentUser)
                    .WithMany(u => u.Following)
                    .HasForeignKey(f => f.CurrentUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries()
                 .Where(e => (e.Entity is CuratedList || e.Entity is Item) && (e.State == EntityState.Added || e.State == EntityState.Modified)).ToList();

            foreach (var entity in entities)
            {
                var currEntity = entity.Entity;
                var currTime = DateTime.UtcNow;

                if (currEntity is CuratedList)
                {
                    //Cast to CuratedList
                    ((CuratedList)currEntity).UpdatedAt = currTime;
                    if (entity.State == EntityState.Added)
                    {
                        ((CuratedList)currEntity).CreatedAt = currTime;
                    }
                }
                else if (currEntity is Item)
                {
                    //Cast to Item
                    ((Item)currEntity).UpdatedAt = currTime;
                    if (entity.State == EntityState.Added)
                    {
                        ((Item)currEntity).CreatedAt = currTime;
                    }
                }
            }
        }
    }
}