using EntityFramework.Entities;

namespace EntityFramework.Commands
{
    public class OrderContentCommands : CommandsBase<OrderContent>
    {
        public OrderContentCommands(AppDbContext dbContext) : base(dbContext) { }


    }
}
