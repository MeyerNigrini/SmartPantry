using System.Net;
using System.Text;
using AwesomeAssertions;
using NUnit.Framework;
using SmartPantry.WebApi.Tests.RecipeControllerTests.Base_Setup;

namespace SmartPantry.WebApi.Tests.RecipeControllerTests
{
    /// <summary>
    /// Integration tests for RecipeController ensuring authenticated recipe CRUD behavior.
    /// </summary>
    [TestFixture]
    public class RecipeControllerTests : TestBase_RecipeControllerTests
    {
        /// <summary>
        /// Adds a recipe for the authenticated user and expects HTTP 200 OK.
        /// </summary>
        [Test]
        public async Task AddRecipeForUser_ShouldReturnOk_WhenRecipeIsValid()
        {
            // Arrange
            var payload = new StringContent(
                "{\"title\":\"Pasta\",\"ingredients\":[\"Noodles\",\"Tomato\"],\"instructions\":[\"Boil\",\"Mix\"]}",
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/recipe/addRecipeForUser", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Attempts to add a recipe with invalid input and expects HTTP 400 BadRequest.
        /// </summary>
        [Test]
        public async Task AddRecipeForUser_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            // Arrange - missing fields
            var payload = new StringContent(
                "{\"title\":\"\"}",
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/recipe/addRecipeForUser", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Gets recipes for authenticated user and expects HTTP 200 OK.
        /// </summary>
        [Test]
        public async Task GetAllRecipesForUser_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange - add a recipe first
            var payload = new StringContent(
                "{\"title\":\"Toast\",\"ingredients\":[\"Bread\"],\"instructions\":[\"Toast it\"]}",
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/recipe/addRecipeForUser", payload);

            // Act
            var response = await _client.GetAsync("/api/recipe/getAllRecipesForUser");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes a recipe for authenticated user and expects 200 OK.
        /// </summary>
        [Test]
        public async Task DeleteRecipeForUser_ShouldReturnOk_WhenRecipeExists()
        {
            // Arrange - Create recipe
            var createPayload = new StringContent(
                "{\"title\":\"Coffee\",\"ingredients\":[\"Beans\"],\"instructions\":[\"Brew\"]}",
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync("/api/recipe/addRecipeForUser", createPayload);
            var createdText = await createResponse.Content.ReadAsStringAsync();

            // Extract recipeId from DB? No — instead GET all recipes and pick the newest
            var getResponse = await _client.GetAsync("/api/recipe/getAllRecipesForUser");
            var json = await getResponse.Content.ReadAsStringAsync();

            var doc = System.Text.Json.JsonDocument.Parse(json);
            var recipeId = doc.RootElement[0].GetProperty("id").GetGuid();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/recipe/deleteRecipeForUser/{recipeId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Attempts to update an existing recipe and expects HTTP 200 OK.
        /// </summary>
        [Test]
        public async Task UpdateRecipeForUser_ShouldReturnOk_WhenRecipeExists()
        {
            // Arrange - create a recipe
            var createPayload = new StringContent(
                "{\"title\":\"Soup\",\"ingredients\":[\"Water\"],\"instructions\":[\"Boil\"]}",
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/recipe/addRecipeForUser", createPayload);

            // Get recipe ID
            var getResponse = await _client.GetAsync("/api/recipe/getAllRecipesForUser");
            var json = await getResponse.Content.ReadAsStringAsync();
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var recipeId = doc.RootElement[0].GetProperty("id").GetGuid();

            var updatePayload = new StringContent(
                "{\"title\":\"Soup Updated\",\"ingredients\":[\"Water\"],\"instructions\":[\"Boil it\"]}",
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PatchAsync($"/api/recipe/updateRecipeForUser/{recipeId}", updatePayload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
