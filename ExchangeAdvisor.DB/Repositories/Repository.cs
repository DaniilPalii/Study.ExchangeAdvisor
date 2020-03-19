using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.DB.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeAdvisor.DB.Repositories
{
    internal class Repository<TEntity> where TEntity : EntityBase
    {
        public Repository(string connectionString)
        {
            this.connectionString = connectionString;
        }

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

        private TResult PerformInDb<TResult>(Func<DbSet<TEntity>, TResult> action)
        {
            using var db = CreateDatabaseContext();

            return action(db.Set<TEntity>());
        }

        private TResult PerformInDbWithSaving<TResult>(Func<DbSet<TEntity>, TResult> action)
        {
            using var db = CreateDatabaseContext();
            var result = action(db.Set<TEntity>());

            db.SaveChanges();

            return result;
        }

        private void PerformInDbWithSaving(Action<DbSet<TEntity>> action)
        {
            using var db = CreateDatabaseContext();
            action(db.Set<TEntity>());

            db.SaveChanges();
        }

        private DatabaseContext CreateDatabaseContext() => new DatabaseContext(connectionString);

        private readonly string connectionString;
    }
}
