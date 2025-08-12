using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;
using SmartPantry.Core.Settings;
using SmartPantry.Services.Services;

namespace SmartPantry.Services.Tests.UserServiceTests.Base_Setup
{
    /// <summary>
    /// Base setup for UserService tests providing mocks and service initialization.
    /// </summary>
    public abstract class TestBase_UserServiceTests
    {
        protected Mock<IUserRepository> _userRepoMock;
        protected Mock<IPasswordService> _passwordServiceMock;
        protected Mock<ILogger<UserService>> _loggerMock;
        protected IOptions<JWTSettings> _jwtOptions;
        protected UserService _service;

        [SetUp]
        public void BaseSetup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _jwtOptions = Options.Create(
                new JWTSettings
                {
                    Key = "ThisIsASecureKeyChangeMe1234567890123456!",
                    Issuer = "TestIssuer",
                    Audience = "TestAudience",
                    ExpiryMinutes = 60,
                }
            );

            _service = new UserService(
                _userRepoMock.Object,
                _passwordServiceMock.Object,
                _loggerMock.Object,
                _jwtOptions
            );
        }
    }
}
