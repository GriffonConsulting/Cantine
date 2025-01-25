using EntityFramework.Entities;

namespace EntityFramework.Commands
{
    public class ClientAccountTransactionHistoryCommands : CommandsBase<ClientAccountTransactionHistory>
    {
        public ClientAccountTransactionHistoryCommands(AppDbContext dbContext) : base(dbContext) { }


    }
}
