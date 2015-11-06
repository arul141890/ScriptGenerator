using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PaymentGateway.Core.Data.DbContexts;
using PaymentGateway.Core.Data.Repositories;

namespace PaymentGateway.Data.Repositories.EntityFramework
{
    public abstract class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly IDbContext Context;
        protected readonly IDbSet<T> Entities;

        protected EfRepository(IDbContext dbContext)
        {
            Context = dbContext;
            Entities = dbContext.Set<T>();
        }

        #region IRepository<T> Members

        public void Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Add(entity);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var entry = Context.Entry(entity);

            // attach the entity if its not already attached
            if (entry.State == EntityState.Detached)
            {
                Entities.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Entities.Attach(entity);

            Entities.Remove(entity);
        }

        public void Delete(object id)
        {
            Delete(Entities.Find(id));
        }

        public T Retrieve(object id)
        {
            return Entities.Find(id);
        }

        public IEnumerable<T> SelectAll()
        {
            return Entities.AsEnumerable();
        }

        public IEnumerable<T> SelectAllPaged(int page, int pageSize)
        {
            return Entities.AsEnumerable().Skip(page*pageSize).Take(pageSize);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return Entities.Where(predicate).AsEnumerable();
        }

        public IEnumerable<T> FindPaged(int page, int pageSize, Expression<Func<T, bool>> predicate)
        {
            return Entities.Where(predicate).AsEnumerable().Skip(page*pageSize).Take(pageSize);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Entities.SingleOrDefault(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Entities.FirstOrDefault(predicate);
        }

        public int Count()
        {
            return Entities.Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Entities.Count(predicate);
        }

        public void Commit()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw dbEx.DbEntityValidationExceptionToException();
            }
        }

        #endregion
    }
}