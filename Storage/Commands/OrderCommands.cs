using EntityFramework.Entities;

namespace EntityFramework.Commands
{
    public class OrderCommands : CommandsBase<Order>
    {
        public OrderCommands(AppDbContext dbContext) : base(dbContext) { }


    }
}
