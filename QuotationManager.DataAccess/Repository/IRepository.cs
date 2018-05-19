using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QuotationManager.DataAccess.Repository
{

    public interface IRepository<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity item);
        Task<TEntity> FindByIdAsync(int id);
        Task<List<TEntity>> GetAsync();
        IEnumerable<TEntity> Get(Func<TEntity, bool> predicate);
        Task RemoveAsync(TEntity item);
        Task UpdateAsync(TEntity item);
        Task<List<TEntity>> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties);
        IEnumerable<TEntity> GetWithInclude(Func<TEntity, bool> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties);
        Task<int> TotalElementsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
