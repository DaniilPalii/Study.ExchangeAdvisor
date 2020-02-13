using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.DB.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeAdvisor.DB.Repositories.Implementations
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        public async Task<TEntity> GetAsync(int id)
        {
            return await PerformInDbAsync(set => set.FindAsync(id));
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

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await PerformInDbWithSavingAsync(set => set.Update(entity).Entity);
        }

        public async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            await PerformInDbWithSavingAsync(set => set.UpdateRange(entities));
        }

        public async Task RemoveAsync(TEntity entity)
        {
            await PerformInDbWithSavingAsync(set => set.Remove(entity));
        }

        public async Task RemoveAsync(IEnumerable<TEntity> entities)
        {
            await PerformInDbWithSavingAsync(set => set.RemoveRange(entities));
        }

        private static TResult PerformInDb<TResult>(Func<DbSet<TEntity>, TResult> action)
        {
            using (var db = new DatabaseContext())
            {
                return action(db.Set<TEntity>());
            }
        }

        private static async Task<TResult> PerformInDbAsync<TResult>(Func<DbSet<TEntity>, ValueTask<TResult>> asyncAction)
        {
            using (var db = new DatabaseContext())
            {
                return await asyncAction(db.Set<TEntity>());
            }
        }

        private static async Task<TResult> PerformInDbWithSavingAsync<TResult>(Func<DbSet<TEntity>, TResult> action)
        {
            using (var db = new DatabaseContext())
            {
                var result = action(db.Set<TEntity>());

                await db.SaveChangesAsync();

                return result;
            }
        }

        private static async Task PerformInDbWithSavingAsync(Action<DbSet<TEntity>> action)
        {
            using (var db = new DatabaseContext())
            {
                action(db.Set<TEntity>());

                await db.SaveChangesAsync();
            }
        }
    }
}
