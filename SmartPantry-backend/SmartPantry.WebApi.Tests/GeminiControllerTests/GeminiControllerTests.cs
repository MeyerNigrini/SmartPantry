using System.Net;
using System.Text;
using AwesomeAssertions;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.DTOs.Gemini;
using SmartPantry.Core.Exceptions;
using SmartPantry.WebApi.Tests.GeminiControllerTests.Base_Setup;

namespace SmartPantry.WebApi.Tests.GeminiControllerTests
{
    /// <summary>
    /// Integration tests for GeminiController with IGeminiService mocked.
    /// JWT authentication and full real pipeline except external AI calls.
    /// </summary>
    [TestFixture]
    public class GeminiControllerTests : TestBase_GeminiControllerTests
    {
        /// <summary>
        /// generate-recipe: returns 200 when ingredients exist and AI returns JSON.
        /// </summary>
        [Test]
        public async Task GenerateRecipe_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var productIds = $"[\"{Guid.NewGuid()}\" ]";

            _factory.GeminiServiceMock
                .Setup(x => x.GetIngredientsFromFoodProducts(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<string> { "Pasta - 500g - Fatti's - Dry" });

            _factory.GeminiServiceMock
                .Setup(x => x.GetGeminiResponse(It.IsAny<string>()))
                .ReturnsAsync("{\"title\":\"Pasta\",\"ingredients\":[\"Pasta\"],\"instructions\":[\"Cook\"]}");

            var payload = new StringContent(productIds, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/gemini/generate-recipe", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// generate-recipe: returns 400 when no IDs provided.
        /// </summary>
        [Test]
        public async Task GenerateRecipe_ShouldReturnBadRequest_WhenIdsAreMissing()
        {
            // Arrange
            var payload = new StringContent("[]", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/gemini/generate-recipe", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// generate-recipe: returns 404 when no ingredients found.
        /// </summary>
        [Test]
        public async Task GenerateRecipe_ShouldReturnNotFound_WhenNoIngredientsFound()
        {
            // Arrange
            _factory.GeminiServiceMock
                .Setup(x => x.GetIngredientsFromFoodProducts(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<string>());

            var payload = new StringContent($"[\"{Guid.NewGuid()}\"]", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/gemini/generate-recipe", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// generate-recipe: returns 502 when AI service fails.
        /// </summary>
        [Test]
        public async Task GenerateRecipe_ShouldReturnBadGateway_WhenGeminiFails()
        {
            // Arrange
            _factory.GeminiServiceMock
                .Setup(x => x.GetIngredientsFromFoodProducts(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<string> { "Ingredient" });

            _factory.GeminiServiceMock
                .Setup(x => x.GetGeminiResponse(It.IsAny<string>()))
                .ThrowsAsync(new ExternalServiceException("Service down"));

            var payload = new StringContent($"[\"{Guid.NewGuid()}\"]", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/gemini/generate-recipe", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        }

        /// <summary>
        /// extract-product: returns 200 when valid image uploaded and AI returns data.
        /// </summary>
        [Test]
        public async Task ExtractProduct_ShouldReturnOk_WhenImageIsValid()
        {
            // Arrange
            var imageBytes = Encoding.UTF8.GetBytes("fakeimage");
            var byteArrayContent = new ByteArrayContent(imageBytes);
            byteArrayContent.Headers.Add("Content-Type", "image/png");

            var form = new MultipartFormDataContent
            {
                { byteArrayContent, "image", "test.png" }
            };

            _factory.GeminiServiceMock
                .Setup(x => x.ExtractProductFromImageAsync(
                    It.IsAny<ImagePayload>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProductVisionExtract("Apple", "1kg", "Woolworths", "Fruits", null));

            // Act
            var response = await _client.PostAsync("/api/gemini/extract-product", form);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// extract-product: returns 400 when no file is uploaded.
        /// </summary>
        [Test]
        public async Task ExtractProduct_ShouldReturnBadRequest_WhenImageMissing()
        {
            // Arrange
            var form = new MultipartFormDataContent();

            // Act
            var response = await _client.PostAsync("/api/gemini/extract-product", form);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// extract-product: returns 502 when external AI service fails.
        /// </summary>
        [Test]
        public async Task ExtractProduct_ShouldReturnBadGateway_WhenGeminiFails()
        {
            // Arrange
            var imageBytes = Encoding.UTF8.GetBytes("fakeimage");
            var byteArrayContent = new ByteArrayContent(imageBytes);
            byteArrayContent.Headers.Add("Content-Type", "image/png");

            var form = new MultipartFormDataContent
            {
                { byteArrayContent, "image", "test.png" }
            };

            _factory.GeminiServiceMock
                .Setup(x => x.ExtractProductFromImageAsync(
                    It.IsAny<ImagePayload>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ExternalServiceException("Gemini down"));

            // Act
            var response = await _client.PostAsync("/api/gemini/extract-product", form);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        }
    }
}
