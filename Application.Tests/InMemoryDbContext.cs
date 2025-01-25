using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Tests
{
    public class InMemoryDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase("Database", new InMemoryDatabaseRoot())
                .EnableServiceProviderCaching(false);
        }

        public override void Dispose()
        {
        }

        public void TrulyDispose()
        {
            base.Dispose();
        }
    }
}
