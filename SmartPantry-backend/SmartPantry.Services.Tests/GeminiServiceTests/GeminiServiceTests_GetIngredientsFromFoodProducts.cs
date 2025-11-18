using AwesomeAssertions;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Services.Tests.GeminiServiceTests.Base_Setup;

namespace SmartPantry.Services.Tests.GeminiServiceTests
{
    /// <summary>
    /// Tests for GeminiService.GetIngredientsFromFoodProducts
    /// ensuring correct validation, mapping, and error wrapping.
    /// </summary>
    [TestFixture]
    public class GetIngredientsFromFoodProductsTests : TestBase_GeminiServiceTests
    {
        /// <summary>
        /// Ensures an InvalidInputException is thrown when product list is null.
        /// </summary>
        [Test]
        public async Task GetIngredientsFromFoodProducts_NullList_ThrowsInvalidInputException()
        {
            // Arrange
            List<Guid> productIds = null;

            // Act
            Func<Task> act = async () => await _service.GetIngredientsFromFoodProducts(productIds);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Ensures an InvalidInputException is thrown when product list is empty.
        /// </summary>
        [Test]
        public async Task GetIngredientsFromFoodProducts_EmptyList_ThrowsInvalidInputException()
        {
            // Arrange
            var productIds = new List<Guid>();

            // Act
            Func<Task> act = async () => await _service.GetIngredientsFromFoodProducts(productIds);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Ensures products are correctly mapped into formatted strings.
        /// </summary>
        [Test]
        public async Task GetIngredientsFromFoodProducts_ValidIds_MapsCorrectly()
        {
            // Arrange
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var products = new List<FoodProductEntity>
            {
                new FoodProductEntity
                {
                    Id = id1,
                    ProductName = "Milk",
                    Quantity = "1L",
                    Brands = "Clover",
                    Category = "Dairy"
                },
                new FoodProductEntity
                {
                    Id = id2,
                    ProductName = "Bread",
                    Quantity = "1",
                    Brands = "Albany",
                    Category = "Bakery"
                }
            };

            _foodRepoMock
                .Setup(r => r.GetFoodProductsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(products);

            var ids = new List<Guid> { id1, id2 };

            // Act
            var result = await _service.GetIngredientsFromFoodProducts(ids);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain("Milk - 1L - Clover - Dairy");
            result.Should().Contain("Bread - 1 - Albany - Bakery");
        }

        /// <summary>
        /// Ensures repository exceptions are wrapped into PersistenceException.
        /// </summary>
        [Test]
        public async Task GetIngredientsFromFoodProducts_RepositoryThrows_WrapsIntoPersistenceException()
        {
            // Arrange
            _foodRepoMock
                .Setup(r => r.GetFoodProductsByIdsAsync(It.IsAny<List<Guid>>()))
                .ThrowsAsync(new Exception("DB fail"));

            var ids = new List<Guid> { Guid.NewGuid() };

            // Act
            Func<Task> act = async () => await _service.GetIngredientsFromFoodProducts(ids);

            // Assert
            await act.Should().ThrowAsync<PersistenceException>();
        }
    }
}
