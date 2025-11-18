using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.WebApi.Tests.GeminiControllerTests.Base_Setup
{
    /// <summary>
    /// Custom WebApplicationFactory overriding ONLY IGeminiService
    /// so the GeminiController can be tested without hitting real AI APIs.
    /// </summary>
    public class CustomGeminiWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IGeminiService> GeminiServiceMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real GeminiService registration
                var descriptor = services.Single(s =>
                    s.ServiceType == typeof(IGeminiService)
                );
                services.Remove(descriptor);

                // Replace with mock
                services.AddScoped(_ => GeminiServiceMock.Object);
            });
        }
    }
}
