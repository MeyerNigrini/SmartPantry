using System.Net;
using AwesomeAssertions;
using Moq.Protected;
using NUnit.Framework;
using SmartPantry.Core.DTOs.Gemini;
using SmartPantry.Core.Exceptions;
using SmartPantry.Services.Tests.GeminiServiceTests.Base_Setup;

namespace SmartPantry.Services.Tests.GeminiServiceTests
{
    /// <summary>
    /// Tests for ExtractProductFromImageAsync covering validation,
    /// HTTP failures, JSON parsing errors, and successful extraction.
    /// </summary>
    [TestFixture]
    public class GeminiServiceTests_ExtractProductFromImage : TestBase_GeminiServiceTests
    {
        // ----------------------------
        // VALIDATION TESTS
        // ----------------------------

        /// <summary>
        /// Verifies method throws when image is null.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_ImageNull_ThrowsInvalidInputException()
        {
            // Arrange
            ImagePayload image = null;

            // Act
            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            // Assert
            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies method throws when image bytes are null.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_BytesNull_ThrowsInvalidInputException()
        {
            // Arrange
            var image = new ImagePayload(null, "image/png");

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies method throws when image bytes are empty.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_BytesEmpty_ThrowsInvalidInputException()
        {
            var image = new ImagePayload(Array.Empty<byte>(), "image/png");

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies method throws for unsupported MIME type.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_UnsupportedMime_ThrowsInvalidInputException()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/tiff");

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<InvalidInputException>();
        }

        /// <summary>
        /// Verifies method throws when image exceeds max size.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_TooLarge_ThrowsInvalidInputException()
        {
            var tooBig = new byte[_settings.Value.MaxImageBytes + 1];
            var image = new ImagePayload(tooBig, "image/png");

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<InvalidInputException>();
        }

        // ----------------------------
        // HTTP FAILURE TESTS
        // ----------------------------

        /// <summary>
        /// Verifies HTTP error status throws ExternalServiceException.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_HttpError_ThrowsExternalServiceException()
        {
            // Arrange
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");
            SetupHttpResponse(HttpStatusCode.BadRequest, "{ \"error\": \"bad request\" }");

            // Act
            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            // Assert
            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies timeout creates ExternalServiceException.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_Timeout_ThrowsExternalServiceException()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");

            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Throws(new TaskCanceledException());

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies HttpRequestException creates ExternalServiceException.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_HttpRequestException_ThrowsExternalServiceException()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");

            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Throws(new HttpRequestException("network fail"));

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        // ----------------------------
        // JSON FAILURE TESTS
        // ----------------------------

        /// <summary>
        /// Verifies missing candidates throws ExternalServiceException.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_NoCandidates_ThrowsExternalServiceException()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");

            string json = """
            { "candidates": [] }
            """;

            SetupHttpResponse(HttpStatusCode.OK, json);

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies empty text content throws ExternalServiceException.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_EmptyText_ThrowsExternalServiceException()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");

            string json = """
            {
                "candidates": [
                    { "content": { "parts": [ { "text": "" } ] } }
                ]
            }
            """;

            SetupHttpResponse(HttpStatusCode.OK, json);

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies parsed JSON resulting in null model throws ExternalServiceException.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_DeserializesToNull_ThrowsExternalServiceException()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");

            string json = """
            {
                "candidates": [
                    { "content": { "parts": [ { "text": "{}" } ] } }
                ]
            }
            """;

            SetupHttpResponse(HttpStatusCode.OK, json);

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies AI returns empty fields → ExternalServiceException.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_EmptyFields_ThrowsExternalServiceException()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");

            string json = """
            {
                "candidates": [
                    { "content": { "parts": [ { "text": "{ \"ProductName\": \"\" }" } ] } }
                ]
            }
            """;

            SetupHttpResponse(HttpStatusCode.OK, json);

            Func<Task> act = async () => await _service.ExtractProductFromImageAsync(image, "instr");

            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        // ----------------------------
        // SUCCESS TEST
        // ----------------------------

        /// <summary>
        /// Verifies successful JSON → ProductVisionExtract mapping.
        /// </summary>
        [Test]
        public async Task ExtractProductFromImageAsync_ValidResponse_ReturnsProductVisionExtract()
        {
            var image = new ImagePayload(new byte[] { 1, 2, 3 }, "image/png");

            string json = """
            {
                "candidates": [
                    {
                        "content": {
                            "parts": [
                                {
                                    "text": "{ \"ProductName\": \"Milk\", \"Quantity\": \"1L\", \"Brand\": \"Clover\", \"Category\": \"Dairy\", \"ExpirationDate\": \"2025-12-01\" }"
                                }
                            ]
                        }
                    }
                ]
            }
            """;

            SetupHttpResponse(HttpStatusCode.OK, json);

            // Act
            var result = await _service.ExtractProductFromImageAsync(image, "instr");

            // Assert
            result.ProductName.Should().Be("Milk");
            result.Quantity.Should().Be("1L");
            result.Brand.Should().Be("Clover");
            result.Category.Should().Be("Dairy");
            result.ExpirationDate.Should().Be("2025-12-01");
        }
    }
}
