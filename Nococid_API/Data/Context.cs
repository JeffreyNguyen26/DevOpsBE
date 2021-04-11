using Microsoft.EntityFrameworkCore;
using Nococid_API.Infrastrctures;
using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nococid_API.Data
{
    public interface IContext<T> where T : TableBase
    {
        T Add(T model);
        bool Any(Expression<Func<T, bool>> predicate);
        int Count(Expression<Func<T, bool>> predicate);
        void DeleteAll(IEnumerable<T> entities);
        IList<T> GetAll();
        IList<T> GetAll(Expression<Func<T, bool>> predicate);
        IList<T> GetAll(IQueryable<T> query);
        T GetOne(Guid id);
        T GetOne(IQueryable<T> query);
        T GetOne(Expression<Func<T, bool>> predicate);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        int SaveChanges();
        IQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> predicate);
        void Remove(T model);
    }
    public class Context<T> : IContext<T> where T : TableBase
    {
        private readonly NococidContext NococidContext;
        private readonly DbSet<T> dbSet;

        public Context(NococidContext NococidContext)
        {
            this.NococidContext = NococidContext;
            dbSet = this.NococidContext.Set<T>();
        }

        public T Add(T model)
        {
            return dbSet.AddAsync(model).GetAwaiter().GetResult().Entity;
        }

        public IList<T> GetAll()
        {
            return dbSet.ToListAsync().GetAwaiter().GetResult();
        }

        public IList<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Where(predicate).ToListAsync().GetAwaiter().GetResult();
        }

        public IList<T> GetAll(IQueryable<T> query)
        {
            return query.ToListAsync().GetAwaiter().GetResult();
        }

        public T GetOne(Guid id)
        {
            return dbSet.FindAsync(id).GetAwaiter().GetResult();
        }

        public T GetOne(IQueryable<T> query)
        {
            return query.FirstOrDefaultAsync().GetAwaiter().GetResult();
        }

        public T GetOne(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Where(predicate).FirstOrDefaultAsync().GetAwaiter().GetResult();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Where(predicate);
        }

        public int SaveChanges()
        {
            return NococidContext.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return dbSet.AnyAsync(predicate).GetAwaiter().GetResult();
        }

        public void DeleteAll(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            SaveChanges();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return dbSet.CountAsync(predicate).GetAwaiter().GetResult();
        }

        public void Remove(T model)
        {
            dbSet.Remove(model);
        }

        public IQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> predicate)
        {
            return dbSet.Select(predicate);
        }
    }
}
