using EntityFramework.Entities;
using EntityFramework.Queries;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Commands
{
    public class ClientAccountQueries : QueriesBase<ClientAccount>
    {
        public ClientAccountQueries(AppDbContext dbContext) : base(dbContext) { }


        public Task<ClientAccount?> GetByClientId(Guid clientId, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<ClientAccount>().FirstOrDefaultAsync(ca => ca.ClientId == clientId, cancellationToken);
        }
    }
}
