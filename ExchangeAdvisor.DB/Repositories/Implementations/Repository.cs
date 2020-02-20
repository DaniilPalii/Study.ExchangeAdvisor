using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.DB.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeAdvisor.DB.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        public TEntity Get(int id)
        {
            return PerformInDb(set => set.Find(id));
        }

        public ICollection<TEntity> GetBy(Func<TEntity, bool> predicate)
        {
            return PerformInDb(set =>
                set.AsQueryable()
                    .Where(predicate)
                    .ToArray());
        }

        public ICollection<TEntity> GetAll()
        {
            return PerformInDb(set => set.AsEnumerable().ToArray());
        }

        public TEntity Update(TEntity entity)
        {
            return PerformInDbWithSaving(set => set.Update(entity).Entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            PerformInDbWithSaving(set => set.UpdateRange(entities));
        }

        public void Remove(TEntity entity)
        {
            PerformInDbWithSaving(set => set.Remove(entity));
        }

        public void Remove(IEnumerable<TEntity> entities)
        {
            PerformInDbWithSaving(set => set.RemoveRange(entities));
        }

        private static TResult PerformInDb<TResult>(Func<DbSet<TEntity>, TResult> action)
        {
            using var db = new DatabaseContext();

            return action(db.Set<TEntity>());
        }

        private static TResult PerformInDbWithSaving<TResult>(Func<DbSet<TEntity>, TResult> action)
        {
            using var db = new DatabaseContext();
            var result = action(db.Set<TEntity>());

            db.SaveChanges();

            return result;
        }

        private static void PerformInDbWithSaving(Action<DbSet<TEntity>> action)
        {
            using var db = new DatabaseContext();
            action(db.Set<TEntity>());

            db.SaveChanges();
        }
    }
}
