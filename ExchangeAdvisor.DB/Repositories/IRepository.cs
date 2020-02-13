using ExchangeAdvisor.DB.Entities.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeAdvisor.DB.Repositories
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        Task<TEntity> GetAsync(int id);

        ICollection<TEntity> GetBy(Func<TEntity, bool> predicate);

        ICollection<TEntity> GetAll();

        Task<TEntity> UpdateAsync(TEntity entity);

        Task UpdateAsync(IEnumerable<TEntity> entities);

        Task RemoveAsync(TEntity entity);

        Task RemoveAsync(IEnumerable<TEntity> entities);
    }
}