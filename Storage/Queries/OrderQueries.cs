using EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Queries
{
    public class OrderQueries : QueriesBase<Order>
    {
        public OrderQueries(AppDbContext dbContext) : base(dbContext) { }

        public Task<Order?> GetOrderByIdWithOrderContentAsync(Guid orderId, Guid clientId, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<Order>().Include(c => c.OrderContents).Where(c => c.Id == orderId && c.ClientId == clientId).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
