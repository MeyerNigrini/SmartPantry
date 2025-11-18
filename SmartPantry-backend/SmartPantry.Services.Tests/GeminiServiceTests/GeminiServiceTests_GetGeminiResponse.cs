using System.Net;
using AwesomeAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SmartPantry.Core.Exceptions;

namespace SmartPantry.Services.Tests.GeminiServiceTests
{
    [TestFixture]
    public class GeminiServiceTests_GetGeminiResponse : Base_Setup.TestBase_GeminiServiceTests
    {
        /// <summary>
        /// Verifies GetGeminiResponse returns the candidate text when Gemini succeeds.
        /// </summary>
        [Test]
        public async Task GetGeminiResponse_ValidPrompt_ReturnsText()
        {
            // Arrange
            var responseJson =
                """
                {
                    "candidates": [
                        {
                            "content": {
                                "parts": [
                                    { "text": "Hello World" }
                                ]
                            }
                        }
                    ]
                }
                """;

            SetupHttpResponse(HttpStatusCode.OK, responseJson);

            // Act
            var result = await _service.GetGeminiResponse("test prompt");

            // Assert
            result.Should().Be("Hello World");
        }

        /// <summary>
        /// Verifies GetGeminiResponse wraps unexpected JSON structure in ExternalServiceException.
        /// </summary>
        [Test]
        public async Task GetGeminiResponse_InvalidJson_ThrowsExternalServiceException()
        {
            // Arrange
            var invalidJson =
                """
                { "candidates": [] }
                """;

            SetupHttpResponse(HttpStatusCode.OK, invalidJson);

            // Act
            Func<Task> act = async () => await _service.GetGeminiResponse("test");

            // Assert
            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies GetGeminiResponse throws ExternalServiceException when Gemini returns an error status.
        /// </summary>
        [Test]
        public async Task GetGeminiResponse_HttpError_ThrowsExternalServiceException()
        {
            // Arrange
            SetupHttpResponse(HttpStatusCode.BadRequest, "{ \"error\": \"bad request\" }");

            // Act
            Func<Task> act = async () => await _service.GetGeminiResponse("test");

            // Assert
            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies GetGeminiResponse handles request timeout properly.
        /// </summary>
        [Test]
        public async Task GetGeminiResponse_Timeout_ThrowsExternalServiceException()
        {
            // Arrange
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException("timeout"));

            // Act
            Func<Task> act = async () => await _service.GetGeminiResponse("test");

            // Assert
            await act.Should().ThrowAsync<ExternalServiceException>();
        }

        /// <summary>
        /// Verifies GetGeminiResponse handles HttpRequestException properly.
        /// </summary>
        [Test]
        public async Task GetGeminiResponse_HttpRequestException_ThrowsExternalServiceException()
        {
            // Arrange
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("network fail"));

            // Act
            Func<Task> act = async () => await _service.GetGeminiResponse("test");

            // Assert
            await act.Should().ThrowAsync<ExternalServiceException>();
        }
    }
}
