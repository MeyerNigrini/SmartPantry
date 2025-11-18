using System.Net;
using System.Text;
using AwesomeAssertions;
using NUnit.Framework;
using SmartPantry.WebApi.Tests.FoodProductControllerTests.Base_Setup;

namespace SmartPantry.WebApi.Tests.FoodProductControllerTests
{
    /// <summary>
    /// Full integration tests for FoodProductController using real pipeline + JWT auth.
    /// </summary>
    [TestFixture]
    public class FoodProductControllerTests : TestBase_FoodProductControllerTests
    {
        /// <summary>
        /// Verifies adding a valid food product returns HTTP 200 OK.
        /// </summary>
        [Test]
        public async Task AddFoodProductForUser_ShouldReturnOk_WhenProductIsValid()
        {
            // Arrange
            var payload = new StringContent(
                """
                {
                    "productName": "Milk",
                    "quantity": "1L",
                    "brands": "Clover",
                    "category": "Dairy & Eggs",
                    "expirationDate": "2030-01-01"
                }
                """,
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/foodproduct/addFoodProductForUser", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Verifies invalid model returns HTTP 400.
        /// </summary>
        [Test]
        public async Task AddFoodProductForUser_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            // Arrange — missing required fields
            var payload = new StringContent(
                "{\"productName\":\"\"}",
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/foodproduct/addFoodProductForUser", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Fetches all products for authenticated user and expects HTTP 200 OK.
        /// </summary>
        [Test]
        public async Task GetAllFoodProductsForUser_ShouldReturnOk_WhenAuthenticated()
        {
            // Arrange — add a product so list isn't empty
            var payload = new StringContent(
                """
                {
                    "productName": "Bread",
                    "quantity": "1 loaf",
                    "brands": "Albany",
                    "category": "Bakery",
                    "expirationDate": "2030-01-01"
                }
                """,
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/foodproduct/addFoodProductForUser", payload);

            // Act
            var response = await _client.GetAsync("/api/foodproduct/getAllFoodProductsForUser");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes one or more products for authenticated user and returns HTTP 200.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsForUser_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange — add product
            var createPayload = new StringContent(
                """
                {
                    "productName": "Apples",
                    "quantity": "6 pack",
                    "brands": "Woolworths",
                    "category": "Fruits",
                    "expirationDate": "2030-01-01"
                }
                """,
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/foodproduct/addFoodProductForUser", createPayload);

            // Fetch ID from DB via GET
            var getResponse = await _client.GetAsync("/api/foodproduct/getAllFoodProductsForUser");
            var json = await getResponse.Content.ReadAsStringAsync();

            var doc = System.Text.Json.JsonDocument.Parse(json);
            var productId = doc.RootElement[0].GetProperty("id").GetGuid();

            var deletePayload = new StringContent(
                $"{{\"productIds\":[\"{productId}\"]}}",
                Encoding.UTF8,
                "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/api/foodproduct/deleteFoodProductsForUser", UriKind.Relative),
                Content = deletePayload
            };

            // Act
            var deleteResponse = await _client.SendAsync(request);

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Attempts to delete with invalid DTO and expects HTTP 400.
        /// </summary>
        [Test]
        public async Task DeleteFoodProductsForUser_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            // Arrange — missing IDs
            var payload = new StringContent(
                "{\"productIds\":[]}",
                Encoding.UTF8,
                "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/api/foodproduct/deleteFoodProductsForUser", UriKind.Relative),
                Content = payload
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
