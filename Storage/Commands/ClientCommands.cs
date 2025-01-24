using EntityFramework.Entities;

namespace EntityFramework.Commands
{
    public class ClientCommands : CommandsBase<Client>
    {
        public ClientCommands(AppDbContext dbContext) : base(dbContext) { }


    }
}
