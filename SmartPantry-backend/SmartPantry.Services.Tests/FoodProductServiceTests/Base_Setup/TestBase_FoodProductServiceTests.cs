using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Services.Services;

namespace SmartPantry.Services.Tests.FoodProductServiceTests.Base_Setup
{
    /// <summary>
    /// Base setup for FoodProductService tests providing shared mocks and service initialization.
    /// </summary>
    public abstract class TestBase_FoodProductServiceTests
    {
        protected Mock<IFoodProductRepository> _repoMock;
        protected Mock<ILogger<FoodProductService>> _loggerMock;
        protected FoodProductService _service;

        [SetUp]
        public void BaseSetup()
        {
            _repoMock = new Mock<IFoodProductRepository>();
            _loggerMock = new Mock<ILogger<FoodProductService>>();

            _service = new FoodProductService(
                _repoMock.Object,
                _loggerMock.Object
            );
        }
    }
}
