using EntityFramework.Entities;
using EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EntityFramework.Queries
{
    public abstract class QueriesBase<TEntity> : IDisposable where TEntity : class, IEntity
    {
        protected AppDbContext DbContext { get; }

        protected QueriesBase(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<TEntity> GetFilteredWithInclude(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            string? sortPropertyName = null,
            int? sortOrder = null)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            query = query.Where(predicate);

            if (!string.IsNullOrWhiteSpace(sortPropertyName) && HashProperty(sortPropertyName))
            {
                query = query.OrderByDynamic(sortPropertyName, sortOrder);
            }

            return query;
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public Task<TEntity[]> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return DbContext.Set<TEntity>().ToArrayAsync(cancellationToken);
        }

        public Task<TEntity[]> GetAllByPredicateAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return DbContext.Set<TEntity>().Where(predicate).ToArrayAsync(cancellationToken);
        }

        public ValueTask<TEntity> GetById(params object[] keyValues) => DbContext.Set<TEntity>().FindAsync(keyValues);

        public ValueTask<TEntity> GetById(object[] keyValues, CancellationToken cancellationToken = default) =>
             DbContext.Set<TEntity>().FindAsync(keyValues, cancellationToken);

        public bool IsExist(Expression<Func<TEntity, bool>> predicate) => DbContext.Set<TEntity>().Any(predicate);

        public async Task<TEntity[]> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<TEntity>().Where(predicate).ToArrayAsync(cancellationToken);
        }

        public IQueryable<TEntity> AllAsQueryable()
        {
            return DbContext.Set<TEntity>().AsQueryable();
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbContext.Set<TEntity>().CountAsync(predicate);
        }

        public async Task<TResult[]> GetFilteredPropertiesAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<TEntity>().Where(predicate).Select(selector).ToArrayAsync(cancellationToken);
        }

        private static bool HashProperty(string sortPropertyName) =>
            PropertyHelper.GetPropertyName(PropertyHelper.GetProperty<TEntity>(sortPropertyName)) != null;

        #region IDisposable Support
        private bool _isDisposed = false;

        protected void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    DbContext?.Dispose();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
