using AwesomeAssertions;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.DTOs.FoodProduct;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Enums;
using SmartPantry.Services.Tests.FoodProductServiceTests.Base_Setup;

namespace SmartPantry.Services.Tests.FoodProductServiceTests
{
    /// <summary>
    /// Tests for FoodProductService ensuring validation, mapping, and repository calls behave correctly.
    /// </summary>
    [TestFixture]
    public class FoodProductServiceTests : TestBase_FoodProductServiceTests
    {
        // --------------------------------------------------------------------
        // AddFoodProductForUserAsync
        // --------------------------------------------------------------------

        /// <summary>
        /// Verifies AddFoodProductForUserAsync throws when DTO is null.
        /// </summary>
        [Test]
        public async Task AddFoodProductForUserAsync_NullDto_ThrowsInvalidInputException()
        {
            // Arrange
            FoodProductAddDTO dto = null;
            var userId = Guid.NewGuid();

            // Act
            Func<Task> act = async () =>
                await _service.AddFoodProductForUserAsync(dto, userId);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies AddFoodProductForUserAsync throws when userId is empty.
        /// </summary>
        [Test]
        public async Task AddFoodProductForUserAsync_EmptyUserId_ThrowsInvalidInputException()
        {
            // Arrange
            var dto = new FoodProductAddDTO
            {
                ProductName = "Milk",
                Quantity = "1L",
                Category = "Dairy",
                ExpirationDate = DateTime.UtcNow
            };

            // Act
            Func<Task> act = async () =>
                await _service.AddFoodProductForUserAsync(dto, Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies AddFoodProductForUserAsync calls repository with a correctly mapped entity.
        /// </summary>
        [Test]
        public async Task AddFoodProductForUserAsync_ValidInput_CallsRepositoryWithMappedEntity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expiration = DateTime.UtcNow.AddDays(5);

            var dto = new FoodProductAddDTO
            {
                ProductName = "Apples",
                Quantity = "6",
                Brands = "BrandA",
                Category = "Fruit",
                ExpirationDate = expiration
            };

            FoodProductEntity capturedEntity = null;

            _repoMock.Setup(r => r.AddFoodProductForUserAsync(It.IsAny<FoodProductEntity>()))
                     .Callback<FoodProductEntity>(e => capturedEntity = e)
                     .Returns(Task.CompletedTask);

            // Act
            await _service.AddFoodProductForUserAsync(dto, userId);

            // Assert
            capturedEntity.Should().NotBeNull();
            capturedEntity.UserID.Should().Be(userId);
            capturedEntity.ProductName.Should().Be("Apples");
            capturedEntity.Quantity.Should().Be("6");
            capturedEntity.Brands.Should().Be("BrandA");
            capturedEntity.Category.Should().Be("Fruit");
            capturedEntity.ExpirationDate.Should().Be(expiration.Date);  // date trimmed
            capturedEntity.AddedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Verifies AddFoodProductForUserAsync wraps repository failures in ApplicationException.
        /// </summary>
        [Test]
        public async Task AddFoodProductForUserAsync_RepoThrows_ThrowsApplicationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new FoodProductAddDTO
            {
                ProductName = "Bread",
                Quantity = "1",
                Category = "Bakery",
                ExpirationDate = DateTime.UtcNow
            };

            _repoMock.Setup(r => r.AddFoodProductForUserAsync(It.IsAny<FoodProductEntity>()))
                     .ThrowsAsync(new Exception("DB failed"));

            // Act
            Func<Task> act = async () =>
                await _service.AddFoodProductForUserAsync(dto, userId);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>();
        }

        // --------------------------------------------------------------------
        // GetAllFoodProductsForUserAsync
        // --------------------------------------------------------------------

        /// <summary>
        /// Verifies GetAllFoodProductsForUserAsync throws on empty userId.
        /// </summary>
        [Test]
        public async Task GetAllFoodProductsForUserAsync_EmptyUserId_ThrowsInvalidInputException()
        {
            // Act
            Func<Task> act = async () =>
                await _service.GetAllFoodProductsForUserAsync(Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies GetAllFoodProductsForUserAsync wraps repository errors in PersistenceException.
        /// </summary>
        [Test]
        public async Task GetAllFoodProductsForUserAsync_RepoThrows_ThrowsPersistenceException()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllFoodProductsByUserIdAsync(It.IsAny<Guid>()))
                     .ThrowsAsync(new Exception("DB error"));

            // Act
            Func<Task> act = async () =>
                await _service.GetAllFoodProductsForUserAsync(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<PersistenceException>();
        }

        /// <summary>
        /// Verifies GetAllFoodProductsForUserAsync maps entities to DTOs and computes status correctly.
        /// </summary>
        [Test]
        public async Task GetAllFoodProductsForUserAsync_ValidUser_MapsEntitiesCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;

            var products = new List<FoodProductEntity>
            {
                new FoodProductEntity
                {
                    Id = Guid.NewGuid(),
                    UserID = userId,
                    ProductName = "Milk",
                    Quantity = "1L",
                    Brands = "BrandX",
                    Category = "Dairy",
                    ExpirationDate = today.AddDays(-1), // expired
                    AddedDate = today
                },
                new FoodProductEntity
                {
                    Id = Guid.NewGuid(),
                    UserID = userId,
                    ProductName = "Bread",
                    Quantity = "2",
                    Brands = "BrandY",
                    Category = "Bakery",
                    ExpirationDate = today.AddDays(2), // expiring
                    AddedDate = today
                },
                new FoodProductEntity
                {
                    Id = Guid.NewGuid(),
                    UserID = userId,
                    ProductName = "Pasta",
                    Quantity = "1kg",
                    Brands = "BrandZ",
                    Category = "Pantry",
                    ExpirationDate = today.AddDays(20), // fresh
                    AddedDate = today
                }
            };

            _repoMock.Setup(r => r.GetAllFoodProductsByUserIdAsync(userId))
                     .ReturnsAsync(products);

            // Act
            var result = (await _service.GetAllFoodProductsForUserAsync(userId)).ToList();

            // Assert
            result.Should().HaveCount(3);
            result[0].Status.Should().Be(FoodProductStatus.Expired.ToString());
            result[1].Status.Should().Be(FoodProductStatus.Expiring.ToString());
            result[2].Status.Should().Be(FoodProductStatus.Fresh.ToString());
        }

        // --------------------------------------------------------------------
        // DeleteFoodProductsForUserAsync
        // --------------------------------------------------------------------

        /// <summary>
        /// Verifies DeleteFoodProductsForUserAsync throws when userId is empty.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsForUserAsync_EmptyUserId_ThrowsInvalidInputException()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };

            // Act
            Func<Task> act = async () =>
                await _service.DeleteFoodProductsForUserAsync(ids, Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies DeleteFoodProductsForUserAsync throws when productIds is null or empty.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsForUserAsync_InvalidIds_ThrowsInvalidInputException()
        {
            // Arrange
            List<Guid> ids = null;

            Func<Task> act1 = async () =>
                await _service.DeleteFoodProductsForUserAsync(ids, Guid.NewGuid());

            Func<Task> act2 = async () =>
                await _service.DeleteFoodProductsForUserAsync(new List<Guid>(), Guid.NewGuid());

            // Assert
            await act1.Should().ThrowAsync<InvalidInputException>();
            await act2.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies DeleteFoodProductsForUserAsync throws when repository returns 0.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsForUserAsync_NoMatches_ThrowsInvalidInputException()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };

            _repoMock.Setup(r => r.DeleteFoodProductsByIdsAsync(It.IsAny<Guid>(), ids))
                     .ReturnsAsync(0);

            // Act
            Func<Task> act = async () =>
                await _service.DeleteFoodProductsForUserAsync(ids, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies DeleteFoodProductsForUserAsync returns number of deleted items when repository succeeds.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsForUserAsync_ValidRequest_ReturnsDeletedCount()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            _repoMock.Setup(r => r.DeleteFoodProductsByIdsAsync(It.IsAny<Guid>(), ids))
                     .ReturnsAsync(2);

            // Act
            var result = await _service.DeleteFoodProductsForUserAsync(ids, Guid.NewGuid());

            // Assert
            result.Should().Be(2);
        }

        /// <summary>
        /// Verifies DeleteFoodProductsForUserAsync wraps unexpected exceptions in PersistenceException.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsForUserAsync_RepoThrows_ThrowsPersistenceException()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };

            _repoMock.Setup(r => r.DeleteFoodProductsByIdsAsync(It.IsAny<Guid>(), ids))
                     .ThrowsAsync(new Exception("DB broke"));

            // Act
            Func<Task> act = async () =>
                await _service.DeleteFoodProductsForUserAsync(ids, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<PersistenceException>();
        }
    }
}
