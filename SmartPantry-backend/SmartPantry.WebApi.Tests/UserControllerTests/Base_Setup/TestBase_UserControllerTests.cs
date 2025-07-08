using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace SmartPantry.WebApi.Tests.UserControllerTests.Base_Setup
{
    /// <summary>
    /// Base setup for UserController integration tests providing WebApplicationFactory and HttpClient.
    /// </summary>
    public abstract class TestBase_UserControllerTests
    {
        protected WebApplicationFactory<Program> _factory;
        protected HttpClient _client;

        [SetUp]
        public void BaseSetup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
