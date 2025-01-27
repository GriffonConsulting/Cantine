using EntityFramework.Entities;

namespace EntityFramework.Queries
{
    public class RoleQueries : QueriesBase<Role>
    {
        public RoleQueries(AppDbContext dbContext) : base(dbContext) { }

    }
}
