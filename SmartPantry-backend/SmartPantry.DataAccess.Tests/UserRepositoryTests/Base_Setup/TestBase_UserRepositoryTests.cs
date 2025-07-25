using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SmartPantry.DataAccess.Contexts;
using SmartPantry.DataAccess.Repositories;

namespace SmartPantry.DataAccess.Tests.UserRepositoryTests.Base_Setup
{
    /// <summary>
    /// Base setup for UserRepository tests providing InMemory DB context and repository.
    /// </summary>
    public abstract class TestBase_UserRepositoryTests
    {
        protected SmartPantryDbContext _context;
        protected UserRepository _userRepository;
        protected ILogger<UserRepository> _logger = null!;

        [SetUp]
        public void BaseSetup()
        {
            var options = new DbContextOptionsBuilder<SmartPantryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SmartPantryDbContext(options);
            _context.Database.EnsureCreated();

            _logger = NullLogger<UserRepository>.Instance;
            _userRepository = new UserRepository(_context, _logger);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
