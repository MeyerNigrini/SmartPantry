using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SmartPantry.Core.Entities;
using SmartPantry.DataAccess.Tests.RecipeRepositoryTests.Base_Setup;

namespace SmartPantry.DataAccess.Tests.RecipeRepositoryTests
{
    [TestFixture]
    public class RecipeRepositoryTests : TestBase_RecipeRepositoryTests
    {
        // ---------------------------------------------------------------------
        // AddRecipeForUserAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that AddRecipeForUserAsync adds a valid recipe to the database.
        /// </summary>
        [Test]
        public async Task AddRecipeForUserAsync_ValidRecipe_AddsRecipeToDatabase()
        {
            // Arrange
            var recipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                Title = "Pasta",
                Ingredients = "Noodles, Tomato",
                Instructions = "Boil pasta"
            };

            // Act
            await _recipeRepository.AddRecipeForUserAsync(recipe);

            // Assert
            var stored = await _context.Recipes.FirstAsync();
            stored.Title.Should().Be("Pasta");
            stored.UserID.Should().Be(recipe.UserID);
        }

        /// <summary>
        /// Verifies that AddRecipeForUserAsync does nothing when recipe is null.
        /// </summary>
        [Test]
        public async Task AddRecipeForUserAsync_NullRecipe_DoesNothing()
        {
            // Arrange
            RecipeEntity recipe = null;

            // Act
            await _recipeRepository.AddRecipeForUserAsync(recipe);

            // Assert
            var count = await _context.Recipes.CountAsync();
            count.Should().Be(0);
        }

        // ---------------------------------------------------------------------
        // GetAllRecipesByUserIdAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that GetAllRecipesByUserIdAsync returns ordered recipes for a valid user.
        /// </summary>
        [Test]
        public async Task GetAllRecipesByUserIdAsync_UserHasRecipes_ReturnsOrderedRecipes()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var oldRecipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                Title = "Old",
                Ingredients = "A",
                Instructions = "A",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };

            var newRecipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                Title = "New",
                Ingredients = "B",
                Instructions = "B",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Recipes.AddRangeAsync(oldRecipe, newRecipe);
            await _context.SaveChangesAsync();

            // Act
            var result = await _recipeRepository.GetAllRecipesByUserIdAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.First().Title.Should().Be("New");
        }

        /// <summary>
        /// Verifies that GetAllRecipesByUserIdAsync returns empty when given an empty user ID.
        /// </summary>
        [Test]
        public async Task GetAllRecipesByUserIdAsync_EmptyUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act
            var result = await _recipeRepository.GetAllRecipesByUserIdAsync(userId);

            // Assert
            result.Should().BeEmpty();
        }

        // ---------------------------------------------------------------------
        // DeleteRecipeByIdForUserAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that DeleteRecipeByIdForUserAsync deletes a recipe when valid IDs are provided.
        /// </summary>
        [Test]
        public async Task DeleteRecipeByIdForUserAsync_ValidIds_DeletesRecipeAndReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var recipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                Title = "ToDelete",
                Ingredients = "A",
                Instructions = "B"
            };

            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            // Act
            var result = await _recipeRepository.DeleteRecipeByIdForUserAsync(recipe.Id, userId);

            // Assert
            result.Should().BeTrue();
            var exists = await _context.Recipes.AnyAsync(r => r.Id == recipe.Id);
            exists.Should().BeFalse();
        }

        /// <summary>
        /// Verifies that deleting a recipe not owned by the user returns false.
        /// </summary>
        [Test]
        public async Task DeleteRecipeByIdForUserAsync_RecipeNotOwnedByUser_ReturnsFalse()
        {
            // Arrange
            var recipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                Title = "NotYours",
                Ingredients = "A",
                Instructions = "B"
            };

            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            // Act
            var result = await _recipeRepository.DeleteRecipeByIdForUserAsync(recipe.Id, Guid.NewGuid());

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Verifies that deleting with invalid IDs returns false.
        /// </summary>
        [Test]
        public async Task DeleteRecipeByIdForUserAsync_InvalidIds_ReturnsFalse()
        {
            // Act
            var result = await _recipeRepository.DeleteRecipeByIdForUserAsync(Guid.Empty, Guid.Empty);

            // Assert
            result.Should().BeFalse();
        }

        // ---------------------------------------------------------------------
        // UpdateRecipeForUserAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that UpdateRecipeForUserAsync updates only provided fields.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_ValidIdsAndFields_UpdatesRecipeAndReturnsUpdatedEntity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var recipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                Title = "Old",
                Ingredients = "OldIngredients",
                Instructions = "OldInstructions"
            };

            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            var update = new RecipeEntity
            {
                Title = "NewTitle",
                Ingredients = "NewIngredients",
                Instructions = "NewInstructions"
            };

            // Act
            var updated = await _recipeRepository.UpdateRecipeForUserAsync(recipe.Id, userId, update);

            // Assert
            updated.Should().NotBeNull();
            updated.Title.Should().Be("NewTitle");
            updated.Ingredients.Should().Be("NewIngredients");
            updated.Instructions.Should().Be("NewInstructions");
            updated.UpdatedAt.Should().BeAfter(recipe.CreatedAt);
        }

        /// <summary>
        /// Verifies update returns null when recipe does not belong to user.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_RecipeNotOwnedByUser_ReturnsNull()
        {
            // Arrange
            var recipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                Title = "Old",
                Ingredients = "Original ingredients",
                Instructions = "Original instructions"
            };

            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            var update = new RecipeEntity { Title = "New" };

            // Act
            var result = await _recipeRepository.UpdateRecipeForUserAsync(recipe.Id, Guid.NewGuid(), update);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Verifies update returns null when IDs are invalid.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_InvalidIds_ReturnsNull()
        {
            // Arrange
            var update = new RecipeEntity { Title = "New" };

            // Act
            var result = await _recipeRepository.UpdateRecipeForUserAsync(Guid.Empty, Guid.Empty, update);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Verifies that unprovided fields are not overwritten.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUserAsync_UnprovidedFields_NotOverwritten()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var recipe = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                Title = "OriginalTitle",
                Ingredients = "OriginalIngredients",
                Instructions = "OriginalInstructions"
            };

            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            var update = new RecipeEntity
            {
                Title = "UpdatedTitle",
                Ingredients = null,
                Instructions = " "
            };

            // Act
            var updated = await _recipeRepository.UpdateRecipeForUserAsync(recipe.Id, userId, update);

            // Assert
            updated.Title.Should().Be("UpdatedTitle");
            updated.Ingredients.Should().Be("OriginalIngredients");
            updated.Instructions.Should().Be("OriginalInstructions");
        }
    }
}
