using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartPantry.Core.Settings;
using SmartPantry.Services.External;
using SmartPantry.Core.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace SmartPantry.Services.Tests.GeminiServiceTests.Base_Setup
{
    public abstract class TestBase_GeminiServiceTests
    {
        protected Mock<HttpMessageHandler> _httpHandlerMock;
        protected HttpClient _httpClient;
        protected Mock<ILogger<GeminiService>> _loggerMock;
        protected Mock<IFoodProductRepository> _foodRepoMock;
        protected IOptions<GeminiSettings> _settings;
        protected GeminiService _service;

        [SetUp]
        public void BaseSetup()
        {
            _httpHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            _httpClient = new HttpClient(_httpHandlerMock.Object);

            _loggerMock = new Mock<ILogger<GeminiService>>();
            _foodRepoMock = new Mock<IFoodProductRepository>();

            _settings = Options.Create(new GeminiSettings
            {
                Model = "gemini-2.5-flash",
                TimeoutSeconds = 5,
                MaxImageBytes = 20 * 1024 * 1024
            });

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "Gemini:ApiKey", "test-api-key" }
                    }
                )
                .Build();

            _service = new GeminiService(
                _httpClient,
                config,
                _settings,
                _loggerMock.Object,
                _foodRepoMock.Object
            );
        }
        protected void SetupHttpResponse(HttpStatusCode status, string json)
        {
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
        }
    }
}
