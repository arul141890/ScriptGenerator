// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The Repository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Data
{
    public interface IRepository<TEntity>
    {
        #region Public Methods and Operators

        void Commit();

        int Count();

        int Count(Expression<Func<TEntity, bool>> predicate);

        void Create(TEntity entity);

        void Delete(TEntity entity);

        void Delete(object id);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> FindPaged(int page, int pageSize, Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, long>> columnPredicate);

        IQueryable<TEntity> Query();

        TEntity Retrieve(object id);

        IEnumerable<TEntity> SelectAll();

        IEnumerable<TEntity> SelectAllPaged(int page, int pageSize);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Update(TEntity entity);

        int Sum(Expression<Func<TEntity, int>> predicate);

        IEnumerable<TEntity> FindDistinct(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<string> Query(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, string>> columnPredicate);

        IEnumerable<int> Query(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, int>> columnPredicate);

        DataTable CustomQuery(string query);

        #endregion

    }
}