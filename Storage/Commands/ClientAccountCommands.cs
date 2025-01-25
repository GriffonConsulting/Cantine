using EntityFramework.Entities;

namespace EntityFramework.Commands
{
    public class ClientAccountCommands : CommandsBase<ClientAccount>
    {
        public ClientAccountCommands(AppDbContext dbContext) : base(dbContext) { }


    }
}
