using EntityFramework.Entities;

namespace EntityFramework.Queries
{
    public class OrderContentQueries : QueriesBase<OrderContent>
    {
        public OrderContentQueries(AppDbContext dbContext) : base(dbContext) { }


    }
}
