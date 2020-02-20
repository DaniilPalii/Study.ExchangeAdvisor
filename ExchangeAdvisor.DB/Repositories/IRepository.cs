using ExchangeAdvisor.DB.Entities.Base;
using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.DB.Repositories
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        TEntity Get(int id);

        ICollection<TEntity> GetBy(Func<TEntity, bool> predicate);

        ICollection<TEntity> GetAll();

        TEntity Update(TEntity entity);

        void Update(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void Remove(IEnumerable<TEntity> entities);
    }
}