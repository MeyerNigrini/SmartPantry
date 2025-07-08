using Microsoft.EntityFrameworkCore;
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

        [SetUp]
        public void BaseSetup()
        {
            var options = new DbContextOptionsBuilder<SmartPantryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SmartPantryDbContext(options);
            _context.Database.EnsureCreated();

            _userRepository = new UserRepository(_context);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
