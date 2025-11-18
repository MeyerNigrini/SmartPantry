using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SmartPantry.DataAccess.Contexts;
using SmartPantry.DataAccess.Repositories;

namespace SmartPantry.DataAccess.Tests.RecipeRepositoryTests.Base_Setup
{
    /// <summary>
    /// Base setup for RecipeRepository tests providing InMemory DB context and repository.
    /// </summary>
    public abstract class TestBase_RecipeRepositoryTests
    {
        protected SmartPantryDbContext _context;
        protected RecipeRepository _recipeRepository;
        protected ILogger<RecipeRepository> _logger = null!;

        [SetUp]
        public void BaseSetup()
        {
            var options = new DbContextOptionsBuilder<SmartPantryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new SmartPantryDbContext(options);
            _context.Database.EnsureCreated();

            _logger = NullLogger<RecipeRepository>.Instance;
            _recipeRepository = new RecipeRepository(_context, _logger);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
