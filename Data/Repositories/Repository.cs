// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using Core.Data;
using Data.DbContexts;

namespace Data.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        #region Fields

        protected readonly MyDbContext Context;

        protected readonly IDbSet<T> Entities;

        #endregion

        #region Constructors and Destructors

        public Repository(MyDbContext dbContext)
        {
            this.Context = dbContext;
            this.Entities = this.Context.Set<T>();
        }

        #endregion

        #region Public Methods and Operators

        public void Commit()
        {
            try
            {
                this.Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw dbEx.DbEntityValidationExceptionToException();
            }
        }

        public int Count()
        {
            return this.Entities.Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.Count(predicate);
        }

        public int Sum(Expression<Func<T, int>> predicate)
        {
            return this.Entities.Sum(predicate);
        }

        public void Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.Entities.Add(entity);
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            DbEntityEntry<T> entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.Entities.Attach(entity);
            }

            this.Entities.Remove(entity);
        }

        public void Delete(object id)
        {
            this.Delete(this.Entities.Find(id));
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.Where(predicate).AsEnumerable();
        }

        public IEnumerable<T> FindDistinct(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.Where(predicate).Distinct().AsEnumerable();
        }

        public IEnumerable<T> FindPaged(int page, int pageSize, Expression<Func<T, bool>> predicate)
        {
            return this.Entities.Where(predicate).AsEnumerable().Skip(page * pageSize).Take(pageSize);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.FirstOrDefault(predicate);
        }

        public T LastOrDefault(Expression<Func<T, bool>> predicate, Expression<Func<T, long>> columnPredicate)
        {
            return this.Entities.OrderByDescending(columnPredicate).FirstOrDefault(predicate);
        }
        
        public IQueryable<T> Query()
        {
            return this.Entities;
        }

        public IEnumerable<string> Query(Expression<Func<T, bool>> predicate, Expression<Func<T, string>> columnPredicate)
        {
            return this.Entities.Where(predicate).Select(columnPredicate).Distinct().AsEnumerable();
        }

        public IEnumerable<int> Query(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> columnPredicate)
        {
            return this.Entities.Where(predicate).Select(columnPredicate).Distinct().AsEnumerable();
        }

        public T Retrieve(object id)
        {
            return this.Entities.Find(id);
        }

        public IEnumerable<T> SelectAll()
        {
            return this.Entities.AsEnumerable();
        }

        public IEnumerable<T> SelectAllPaged(int page, int pageSize)
        {
            return this.Entities.AsEnumerable().Skip(page * pageSize).Take(pageSize);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.SingleOrDefault(predicate);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            DbEntityEntry<T> entry = this.Context.Entry(entity);

            // attach the entity if its not already attached
            if (entry.State == EntityState.Detached)
            {
                this.Entities.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }

        public DataTable CustomQuery(string query)
        {
            var retVal = new DataTable();
            retVal.Columns.Add("Date", typeof(DateTime));
            retVal.Columns.Add("Name", typeof(string));
            retVal.Columns.Add("Count", typeof(int));
            retVal.Columns.Add("Text", typeof(string));
            retVal.Columns.Add("Price", typeof(int));


            var Val = this.Context.Database.SqlQuery<KeyAndValues>(query).ToList();

            foreach (var datarow in Val)
            {
                retVal.Rows.Add(datarow.Date, datarow.Name, datarow.Count, datarow.Text, datarow.Price);
            }
            return retVal;
        }

        #endregion
    }

    public class KeyAndValues
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string Text { get; set; }
    }
}