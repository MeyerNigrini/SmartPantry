using AwesomeAssertions;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.DTOs.Recipes;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Services.Tests.RecipeServiceTests.Base_Setup;

namespace SmartPantry.Services.Tests.RecipeServiceTests
{
    /// <summary>
    /// Unit tests for RecipeService verifying validation, mapping,
    /// repository interaction, and exception behavior.
    /// </summary>
    [TestFixture]
    public class RecipeServiceTests : TestBase_RecipeServiceTests
    {
        // --------------------------------------------------------
        // ADD
        // --------------------------------------------------------

        /// <summary>
        /// Verifies AddRecipeForUserAsync saves a mapped recipe when input is valid.
        /// </summary>
        [Test]
        public async Task AddRecipeForUserAsync_ValidInput_CallsRepository()
        {
            // Arrange
            var dto = new RecipeCreateDTO
            {
                Title = "Pasta",
                Ingredients = new List<string> { "Tomato", "Pasta" },
                Instructions = new List<string> { "Boil", "Serve" }
            };

            RecipeEntity? saved = null;
            _repoMock.Setup(r => r.AddRecipeForUserAsync(It.IsAny<RecipeEntity>()))
                     .Callback<RecipeEntity>(r => saved = r)
                     .Returns(Task.CompletedTask);

            var userId = Guid.NewGuid();

            // Act
            await _service.AddRecipeForUserAsync(dto, userId);

            // Assert
            saved.Should().NotBeNull();
            saved.Title.Should().Be("Pasta");
            saved.UserID.Should().Be(userId);
        }

        /// <summary>
        /// Verifies AddRecipeForUserAsync throws when DTO is null.
        /// </summary>
        [Test]
        public async Task AddRecipeForUserAsync_NullDto_ThrowsInvalidInputException()
        {
            // Arrange
            RecipeCreateDTO dto = null!;
            Func<Task> act = async () =>
                await _service.AddRecipeForUserAsync(dto, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies AddRecipeForUserAsync throws when userId is empty.
        /// </summary>
        [Test]
        public async Task AddRecipeForUserAsync_EmptyUserId_ThrowsInvalidInputException()
        {
            // Arrange
            var dto = new RecipeCreateDTO { Title = "Valid" };

            Func<Task> act = async () =>
                await _service.AddRecipeForUserAsync(dto, Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies AddRecipeForUserAsync throws when title is missing.
        /// </summary>
        [Test]
        public async Task AddRecipeForUserAsync_MissingTitle_ThrowsInvalidInputException()
        {
            // Arrange
            var dto = new RecipeCreateDTO { Title = "   " };

            Func<Task> act = async () =>
                await _service.AddRecipeForUserAsync(dto, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        // --------------------------------------------------------
        // GET
        // --------------------------------------------------------

        /// <summary>
        /// Verifies GetAllRecipesForUserAsync returns mapped DTOs.
        /// </summary>
        [Test]
        public async Task GetAllRecipesForUserAsync_ValidUserId_ReturnsMappedRecipes()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var recipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                Title = "Toast",
                Ingredients = "Bread\nButter",
                Instructions = "Toast\nSpread",
                CreatedAt = DateTime.UtcNow
            };

            _repoMock.Setup(r => r.GetAllRecipesByUserIdAsync(userId))
                     .ReturnsAsync(new List<RecipeEntity> { recipe });

            // Act
            var result = await _service.GetAllRecipesForUserAsync(userId);

            // Assert
            var list = result.ToList();
            list.Should().HaveCount(1);
            list[0].Ingredients.Should().BeEquivalentTo(new[] { "Bread", "Butter" });
            list[0].Instructions.Should().BeEquivalentTo(new[] { "Toast", "Spread" });
        }

        /// <summary>
        /// Verifies GetAllRecipesForUserAsync throws for empty user ID.
        /// </summary>
        [Test]
        public async Task GetAllRecipesForUserAsync_EmptyUserId_ThrowsInvalidInputException()
        {
            // Act
            Func<Task> act = async () =>
                await _service.GetAllRecipesForUserAsync(Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        // --------------------------------------------------------
        // DELETE
        // --------------------------------------------------------

        /// <summary>
        /// Verifies DeleteRecipeForUserAsync calls repository when valid.
        /// </summary>
        [Test]
        public async Task DeleteRecipeForUserAsync_ValidRequest_CallsRepository()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _repoMock.Setup(r => r.DeleteRecipeByIdForUserAsync(recipeId, userId))
                     .ReturnsAsync(true);

            // Act
            await _service.DeleteRecipeForUserAsync(recipeId, userId);

            // Assert
            _repoMock.Verify(r => r.DeleteRecipeByIdForUserAsync(recipeId, userId), Times.Once);
        }

        /// <summary>
        /// Verifies DeleteRecipeForUserAsync wraps underlying not-owned errors in PersistenceException.
        /// </summary>
        [Test]
        public async Task DeleteRecipeForUserAsync_RecipeNotOwned_ThrowsPersistenceException()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteRecipeByIdForUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                     .ReturnsAsync(false);

            Func<Task> act = async () =>
                await _service.DeleteRecipeForUserAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<PersistenceException>();
        }

        /// <summary>
        /// Verifies DeleteRecipeForUserAsync throws for invalid IDs.
        /// </summary>
        [Test]
        public async Task DeleteRecipeForUserAsync_InvalidIds_ThrowsInvalidInputException()
        {
            // Act
            Func<Task> act = async () =>
                await _service.DeleteRecipeForUserAsync(Guid.Empty, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        // --------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------

        /// <summary>
        /// Verifies UpdateRecipeForUserAsync calls repository and returns mapped response.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_ValidUpdate_ReturnsUpdatedResponse()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var dto = new RecipeUpdateDTO
            {
                Title = "Updated",
                Ingredients = new List<string> { "Egg", "Salt" },
                Instructions = new List<string> { "Mix", "Cook" }
            };

            var savedEntity = new RecipeEntity
            {
                Id = recipeId,
                UserID = userId,
                Title = "Updated",
                Ingredients = "Egg\nSalt",
                Instructions = "Mix\nCook",
                CreatedAt = DateTime.UtcNow
            };

            _repoMock.Setup(r => r.UpdateRecipeForUserAsync(recipeId, userId, It.IsAny<RecipeEntity>()))
                     .ReturnsAsync(savedEntity);

            // Act
            var result = await _service.UpdateRecipeForUserAsync(recipeId, dto, userId);

            // Assert
            result.Title.Should().Be("Updated");
            result.Ingredients.Should().BeEquivalentTo(new[] { "Egg", "Salt" });
        }

        /// <summary>
        /// Verifies UpdateRecipeForUserAsync throws when repository returns null.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_RecipeNotOwned_ThrowsInvalidInputException()
        {
            // Arrange
            _repoMock.Setup(r => r.UpdateRecipeForUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<RecipeEntity>()))
                     .ReturnsAsync((RecipeEntity?)null);

            var dto = new RecipeUpdateDTO { Title = "X" };

            Func<Task> act = async () =>
                await _service.UpdateRecipeForUserAsync(Guid.NewGuid(), dto, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies UpdateRecipeForUserAsync throws if nothing is being updated.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_NoUpdateFields_ThrowsInvalidInputException()
        {
            // Arrange
            var dto = new RecipeUpdateDTO
            {
                Title = " ",
                Ingredients = new List<string>(),
                Instructions = new List<string>()
            };

            Func<Task> act = async () =>
                await _service.UpdateRecipeForUserAsync(Guid.NewGuid(), dto, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies UpdateRecipeForUserAsync throws for invalid IDs.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_InvalidIds_ThrowsInvalidInputException()
        {
            // Arrange
            var dto = new RecipeUpdateDTO { Title = "Valid" };

            Func<Task> act = async () =>
                await _service.UpdateRecipeForUserAsync(Guid.Empty, dto, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies UpdateRecipeForUserAsync throws when DTO is null.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_NullDto_ThrowsInvalidInputException()
        {
            // Arrange
            RecipeUpdateDTO dto = null!;

            Func<Task> act = async () =>
                await _service.UpdateRecipeForUserAsync(Guid.NewGuid(), dto, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }
    }
}
