﻿using Microsoft.EntityFrameworkCore;
using OnlineShop.Library.Common.Interfaces;
using OnlineShop.Library.Data;

namespace OnlineShop.Library.Common.Repos
{
    public abstract class BaseRepo<T> : IRepo<T> 
        where T : class, IIdentifiable, new() //model type must implement IIdentifiable interface and have constructor without parameters
    {
        public BaseRepo(OrdersDbContext context)
        {
            Context = context;
        }

        public OrdersDbContext Context { get; init; }
        protected DbSet<T> Table { get; set; }

        public async Task<Guid> AddAsync(T entity)
        {
            await Table.AddAsync(entity);
            await SaveChangesAsync();
            return entity.Id;
        }
        public async Task<IEnumerable<Guid>> AddRangeAsync(IEnumerable<T> entities)
        {
            await Table.AddRangeAsync(entities);
            await SaveChangesAsync();
            var result = new List<Guid>(entities.Select(e => e.Id));
            return result;
        }
        public async Task<int> DeleteAsync(Guid id)
        {
            var entity = await GetOneAsync(id);
            if (entity != null)
            {
                Context.Entry(entity).State = EntityState.Deleted;
                return await SaveChangesAsync();
            }
            return 0;
        }
        public async Task<int> DeleteRangeAsync(IEnumerable<Guid> ids)
        {
            int result = 0;
            foreach(var id in ids)
            {
                var affectedRows = await DeleteAsync(id);
                result += affectedRows;
            }
            return result;
        }
        public async Task<int> SaveAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return await SaveChangesAsync();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await Table.ToListAsync();
        public virtual async Task<T> GetOneAsync(Guid id) => await Task.Run(() => Table.FirstOrDefault(entity => entity.Id == id));
        internal async Task<int> SaveChangesAsync() //int => number of affected rows (added, updated, removed, etc.)
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw;
            }
            catch(DbUpdateException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
