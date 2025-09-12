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

                entity.Property(f => f.Barcode).HasColumnType("nvarchar(100)");
                entity.Property(f => f.ProductName).HasColumnType("nvarchar(255)");
                entity.Property(f => f.Quantity).HasColumnType("nvarchar(50)");
                entity.Property(f => f.Brands).HasColumnType("nvarchar(100)");
                entity.Property(f => f.Categories).HasColumnType("nvarchar(100)");
                entity.Property(f => f.AddedDate).HasColumnType("datetime2");
                entity.Property(f => f.ExpirationDate).HasColumnType("date");

                // User Relationship
                entity
                    .HasOne(f => f.User)
                    .WithMany(u => u.FoodProducts)
                    .HasForeignKey(f => f.UserID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // MealSuggestion Table
            modelBuilder.Entity<MealSuggestionEntity>(entity =>
            {
                entity.ToTable("MealSuggestion");

                entity.Property(m => m.SuggestionText).HasColumnType("nvarchar(max)");
                entity.Property(m => m.GeneratedAt).HasColumnType("datetime2");

                // User Relationship
                entity
                    .HasOne(m => m.User)
                    .WithMany(u => u.MealSuggestions)
                    .HasForeignKey(m => m.UserID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // MealSuggestionFoodProduct (Join Table)
            modelBuilder.Entity<MealSuggestionFoodProductEntity>(entity =>
            {
                entity.ToTable("MealSuggestionFoodProduct");

                entity.HasKey(m => new { m.MealSuggestionID, m.FoodProductID });

                entity.Property(m => m.OrderIndex).HasColumnType("int");

                // MealSuggestion Relationship
                entity
                    .HasOne(m => m.MealSuggestion)
                    .WithMany(ms => ms.MealSuggestionFoodProducts)
                    .HasForeignKey(m => m.MealSuggestionID)
                    .OnDelete(DeleteBehavior.NoAction);

                // FoodProduct Relationship
                entity
                    .HasOne(m => m.FoodProduct)
                    .WithMany(fp => fp.MealSuggestionFoodProducts)
                    .HasForeignKey(m => m.FoodProductID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            #endregion
        }

        #region DbSet Properties

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<FoodProductEntity> FoodProducts => Set<FoodProductEntity>();
        public DbSet<MealSuggestionEntity> MealSuggestions => Set<MealSuggestionEntity>();
        public DbSet<MealSuggestionFoodProductEntity> MealSuggestionFoodProducts =>
            Set<MealSuggestionFoodProductEntity>();

        #endregion
    }
}
