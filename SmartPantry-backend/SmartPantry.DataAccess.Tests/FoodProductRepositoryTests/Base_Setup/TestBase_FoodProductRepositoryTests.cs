using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SmartPantry.DataAccess.Contexts;
using SmartPantry.DataAccess.Repositories;

namespace SmartPantry.DataAccess.Tests.FoodProductRepositoryTests.Base_Setup
{
    /// <summary>
    /// Base setup for FoodProductRepository tests providing InMemory DB context and repository.
    /// </summary>
    public abstract class TestBase_FoodProductRepositoryTests
    {
        protected SmartPantryDbContext _context;
        protected FoodProductRepository _foodProductRepository;
        protected ILogger<FoodProductRepository> _logger = null!;

        [SetUp]
        public void BaseSetup()
        {
            var options = new DbContextOptionsBuilder<SmartPantryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new SmartPantryDbContext(options);
            _context.Database.EnsureCreated();

            _logger = NullLogger<FoodProductRepository>.Instance;
            _foodProductRepository = new FoodProductRepository(_context, _logger);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
