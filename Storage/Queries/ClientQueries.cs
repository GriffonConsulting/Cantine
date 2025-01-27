using EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Queries
{
    public class ClientQueries : QueriesBase<Client>
    {
        public ClientQueries(AppDbContext dbContext) : base(dbContext) { }

        public Task<Client?> GetClientAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<Client>().Include(c => c.Role).Where(c => c.Id == id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
