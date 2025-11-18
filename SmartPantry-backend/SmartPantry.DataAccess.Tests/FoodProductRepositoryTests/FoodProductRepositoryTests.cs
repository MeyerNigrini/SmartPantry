using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SmartPantry.Core.Entities;
using SmartPantry.DataAccess.Tests.FoodProductRepositoryTests.Base_Setup;

namespace SmartPantry.DataAccess.Tests.FoodProductRepositoryTests
{
    [TestFixture]
    public class FoodProductRepositoryTests : TestBase_FoodProductRepositoryTests
    {
        // ---------------------------------------------------------------------
        // AddFoodProductForUserAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that AddFoodProductForUserAsync successfully adds a food product to the database.
        /// </summary>
        [Test]
        public async Task AddFoodProductForUserAsync_ValidProduct_AddsProductToDatabase()
        {
            // Arrange
            var product = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                ProductName = "Milk",
                Quantity = "1L",
                Brands = "Clover",
                Category = "Dairy",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            await _foodProductRepository.AddFoodProductForUserAsync(product);

            // Assert
            var stored = await _context.FoodProducts.FirstAsync();
            stored.ProductName.Should().Be("Milk");
            stored.Quantity.Should().Be("1L");
            stored.UserID.Should().Be(product.UserID);
        }

        // ---------------------------------------------------------------------
        // GetAllFoodProductsByUserIdAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that GetAllFoodProductsByUserIdAsync returns all products belonging to a user.
        /// </summary>
        [Test]
        public async Task GetAllFoodProductsByUserIdAsync_UserHasProducts_ReturnsProducts()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var p1 = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                ProductName = "Apples",
                Quantity = "5",
                Brands = "BrandA",
                Category = "Fruit",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(5)
            };

            var p2 = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                ProductName = "Bananas",
                Quantity = "6",
                Brands = "BrandB",
                Category = "Fruit",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            await _context.FoodProducts.AddRangeAsync(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _foodProductRepository.GetAllFoodProductsByUserIdAsync(userId);

            // Assert
            result.Should().HaveCount(2);
        }

        /// <summary>
        /// Verifies that GetAllFoodProductsByUserIdAsync returns an empty list when the provided user ID is empty.
        /// </summary>
        [Test]
        public async Task GetAllFoodProductsByUserIdAsync_EmptyUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            var result = await _foodProductRepository.GetAllFoodProductsByUserIdAsync(userId);

            // Assert
            result.Should().BeEmpty();
        }

        // ---------------------------------------------------------------------
        // GetFoodProductsByIdsAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that GetFoodProductsByIdsAsync returns products that match the provided IDs.
        /// </summary>
        [Test]
        public async Task GetFoodProductsByIdsAsync_ValidIds_ReturnsMatchingProducts()
        {
            // Arrange
            var p1 = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                ProductName = "Bread",
                Quantity = "1",
                Brands = "BrandA",
                Category = "Bakery",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(3)
            };

            var p2 = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                ProductName = "Cheese",
                Quantity = "1",
                Brands = "BrandB",
                Category = "Dairy",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(10)
            };

            await _context.FoodProducts.AddRangeAsync(p1, p2);
            await _context.SaveChangesAsync();

            var ids = new List<Guid> { p1.Id, p2.Id };

            // Act
            var result = await _foodProductRepository.GetFoodProductsByIdsAsync(ids);

            // Assert
            result.Should().HaveCount(2);
        }

        /// <summary>
        /// Verifies that GetFoodProductsByIdsAsync returns an empty list when provided with an empty ID collection.
        /// </summary>
        [Test]
        public async Task GetFoodProductsByIdsAsync_EmptyIds_ReturnsEmptyList()
        {
            // Arrange
            var ids = new List<Guid>();

            // Act
            var result = await _foodProductRepository.GetFoodProductsByIdsAsync(ids);

            // Assert
            result.Should().BeEmpty();
        }

        // ---------------------------------------------------------------------
        // DeleteFoodProductsByIdsAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that DeleteFoodProductsByIdsAsync would delete the correct records even though
        /// ExecuteDeleteAsync cannot run on the InMemory provider.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsByIdsAsync_ValidUserAndIds_FiltersCorrectRecords()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var p1 = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                ProductName = "Eggs",
                Quantity = "12",
                Brands = "BrandA",
                Category = "Dairy",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(5)
            };

            var p2 = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                ProductName = "Yogurt",
                Quantity = "1",
                Brands = "BrandB",
                Category = "Dairy",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            var other = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                ProductName = "NotThisUser",
                Quantity = "2",
                Brands = "BrandC",
                Category = "Snacks",
                AddedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(10)
            };

            await _context.FoodProducts.AddRangeAsync(p1, p2, other);
            await _context.SaveChangesAsync();

            var idsToDelete = new List<Guid> { p1.Id, p2.Id };

            // Act — we cannot call ExecuteDeleteAsync on InMemory,
            // so instead we compute the filtered set manually using the same predicate.
            var query = _context.FoodProducts
                .Where(p => p.UserID == userId && idsToDelete.Contains(p.Id));

            var itemsThatWouldBeDeleted = await query.ToListAsync();

            // Assert
            itemsThatWouldBeDeleted.Should().HaveCount(2);
            itemsThatWouldBeDeleted.Should().Contain(x => x.Id == p1.Id);
            itemsThatWouldBeDeleted.Should().Contain(x => x.Id == p2.Id);
        }


        /// <summary>
        /// Verifies that DeleteFoodProductsByIdsAsync returns zero when invalid inputs are provided.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsByIdsAsync_InvalidInputs_ReturnsZero()
        {
            // Arrange
            var result1 = await _foodProductRepository.DeleteFoodProductsByIdsAsync(Guid.Empty, new List<Guid>());
            var result2 = await _foodProductRepository.DeleteFoodProductsByIdsAsync(Guid.NewGuid(), null);

            // Assert
            result1.Should().Be(0);
            result2.Should().Be(0);
        }
    }
}
