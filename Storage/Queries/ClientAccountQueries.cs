using EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Queries
{
    public class ClientAccountQueries : QueriesBase<ClientAccount>
    {
        public ClientAccountQueries(AppDbContext dbContext) : base(dbContext) { }


        public Task<ClientAccount?> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<ClientAccount>().FirstOrDefaultAsync(ca => ca.ClientId == clientId, cancellationToken);
        }
    }
}
