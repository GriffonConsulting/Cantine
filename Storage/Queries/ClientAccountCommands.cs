using EntityFramework.Entities;
using EntityFramework.Queries;

namespace EntityFramework.Commands
{
    public class ClientAccountQueries : QueriesBase<ClientAccount>
    {
        public ClientAccountQueries(AppDbContext dbContext) : base(dbContext) { }


    }
}
