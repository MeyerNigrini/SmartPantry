using Microsoft.EntityFrameworkCore;
using SmartPantry.Core.Entities;

namespace SmartPantry.DataAccess.Contexts
{
    public class SmartPantryDbContext : DbContext
    {
        public SmartPantryDbContext(DbContextOptions<SmartPantryDbContext> options)
            : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
    }
}