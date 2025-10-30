using Microsoft.EntityFrameworkCore;
using SmartPantry.Core.Entities;

namespace SmartPantry.DataAccess.Contexts
{
    public class SmartPantryDbContext : DbContext
    {
        public SmartPantryDbContext(DbContextOptions<SmartPantryDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Table Configurations

            // User Table
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.FirstName).HasColumnType("nvarchar(50)");
                entity.Property(u => u.LastName).HasColumnType("nvarchar(50)");
                entity.Property(u => u.Email).HasColumnType("nvarchar(100)");
                entity.Property(u => u.PasswordHash).HasColumnType("nvarchar(255)");
                entity.Property(u => u.CreateDate).HasColumnType("datetime2");
            });

            // FoodProduct Table
            modelBuilder.Entity<FoodProductEntity>(entity =>
            {
                entity.ToTable("FoodProduct");

                entity.Property(f => f.ProductName).HasColumnType("nvarchar(255)");
                entity.Property(f => f.Quantity).HasColumnType("nvarchar(50)");
                entity.Property(f => f.Brands).HasColumnType("nvarchar(100)");
                entity.Property(f => f.Category).HasColumnType("nvarchar(100)");
                entity.Property(f => f.AddedDate).HasColumnType("datetime2");
                entity.Property(f => f.ExpirationDate).HasColumnType("date");

                // User Relationship
                entity
                    .HasOne(f => f.User)
                    .WithMany(u => u.FoodProducts)
                    .HasForeignKey(f => f.UserID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Recipe Table
            modelBuilder.Entity<RecipeEntity>(entity =>
            {
                entity.ToTable("Recipe");

                entity.Property(r => r.Title).HasColumnType("nvarchar(200)");
                entity.Property(r => r.IngredientsJson).HasColumnType("nvarchar(max)");
                entity.Property(r => r.Instructions).HasColumnType("nvarchar(max)");
                entity.Property(r => r.CreatedAt).HasColumnType("datetime2");
                entity.Property(r => r.UpdatedAt).HasColumnType("datetime2");

                // User Relationship
                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // RecipeFoodProduct (Join Table)
            modelBuilder.Entity<RecipeFoodProductEntity>(entity =>
            {
                entity.ToTable("RecipeFoodProduct");
                entity.HasKey(rf => new { rf.RecipeID, rf.FoodProductID });
                entity.Property(rf => rf.OrderIndex).HasColumnType("int");

                // Recipe Relationship
                entity.HasOne(rf => rf.Recipe)
                      .WithMany(r => r.RecipeFoodProducts)
                      .HasForeignKey(rf => rf.RecipeID)
                      .OnDelete(DeleteBehavior.NoAction);

                // FoodProduct Relationship
                entity.HasOne(rf => rf.FoodProduct)
                      .WithMany()
                      .HasForeignKey(rf => rf.FoodProductID)
                      .OnDelete(DeleteBehavior.NoAction);
            });
            #endregion
        }

        #region DbSet Properties

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<FoodProductEntity> FoodProducts => Set<FoodProductEntity>();
        public DbSet<RecipeEntity> Recipes => Set<RecipeEntity>();
        public DbSet<RecipeFoodProductEntity> RecipeFoodProducts => Set<RecipeFoodProductEntity>();

        #endregion
    }
}
