using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Services.Services;

namespace SmartPantry.Services.Tests.RecipeServiceTests.Base_Setup
{
    /// <summary>
    /// Base setup class for RecipeService tests providing common mocks and service initialization.
    /// </summary>
    public abstract class TestBase_RecipeServiceTests
    {
        protected Mock<IRecipeRepository> _repoMock;
        protected Mock<ILogger<RecipeService>> _loggerMock;
        protected RecipeService _service;

        [SetUp]
        public void BaseSetup()
        {
            // Arrange base mocks
            _repoMock = new Mock<IRecipeRepository>();
            _loggerMock = new Mock<ILogger<RecipeService>>();

            // Create the service under test
            _service = new RecipeService(
                _repoMock.Object,
                _loggerMock.Object
            );
        }
    }
}
