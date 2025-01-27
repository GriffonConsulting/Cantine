using EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Queries
{
    public class ProductQueries : QueriesBase<Product>
    {
        public ProductQueries(AppDbContext dbContext) : base(dbContext) { }


        public Task<Product[]> GetByProductsIdsAsync(Guid[] productIds, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<Product>().Where(mc => productIds.Contains(mc.Id)).ToArrayAsync(cancellationToken);
        }

        public Task<Product?> GetByProductCode(string productCode, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<Product>().FirstOrDefaultAsync(mc => mc.ProductCode == productCode, cancellationToken);
        }
    }
}
