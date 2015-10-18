// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Core
{
    using System;
    using System.Collections.Generic;

    using CommonUtilitiesModule.Logging;

    using DataAccessModule.Data.Filtering;
    using DataAccessModule.Data.Repositories;
    using DataAccessModule.Data.Sorting;
    using DataAccessModule.Data.Tables;

    using PetaPoco;

    public class Repository : IRepository
    {

        private static readonly ILogger Logger = new Logger(typeof(Repository));



        private readonly Database _db = new Database("ConnectionString");



        public Repository()
        {
            this._db.KeepConnectionAlive = false;
        }



        public void BatchInsert<T>(IList<T> entities) where T : class
        {
            if (entities.Count > 0) this._db.BatchInsert(entities);
        }

        public int Count<T>() where T : class
        {
            var sql = string.Format("SELECT COUNT(*) FROM {0}", typeof(T).Name);
            var count = this._db.ExecuteScalar<int>(sql);

            return count;
        }

        public int Count<T>(Filters filters) where T : class
        {
            var result = filters.ToSqlAndValues();

            var sql = string.Format("SELECT COUNT(*) FROM {0} {1}", typeof(T).Name, result.Sql);
            var count = this._db.ExecuteScalar<int>(sql, result.Values);

            return count;
        }

        public int Delete<T>(T entity) where T : class
        {
            return this._db.Delete(entity);
        }

        public int Delete<T>(Filters filters) where T : class
        {
            var result = filters.ToSqlAndValues();
            return this._db.Delete<T>(result.Sql, result.Values);
        }

        public void ExecuteAtomic(Action action)
        {
            using (var transaction = this._db.GetTransaction())
            {
                action.Invoke();
                transaction.Complete();
            }
        }

        public int ExecuteNonQuery(string query)
        {
            return this._db.Execute(query);
        }

        public IList<T> ExecuteQuery<T>(string query, params object[] values) where T : class
        {
            return this._db.Fetch<T>(query, values);
        }

        public T ExecuteScalar<T>(string query)
        {
            return this._db.ExecuteScalar<T>(query);
        }

        public void Insert<T>(T entity) where T : class
        {
            this._db.Insert(entity);
        }

        public PagedList<T> PagedSelectAll<T>(int page, int pageSize, Filters filters, SortOptions sortOptions)
            where T : class
        {
            if (filters == null)
            {
                filters = new Filters();
            }

            if (sortOptions == null)
            {
                sortOptions = new SortOptions();
            }

            var result = filters.ToSqlAndValues();
            var p = this._db.Page<T>(page, pageSize, result.Sql + sortOptions.ToSql(), result.Values);

            var pagedList = new PagedList<T>();

            pagedList.AddRange(p.Items);
            pagedList.CurrentPage = p.CurrentPage;
            pagedList.TotalPages = p.TotalPages;
            pagedList.TotalItems = p.TotalItems;
            pagedList.PageSize = p.ItemsPerPage;
            
            return pagedList;
        }

        public IList<T> SelectAll<T>(Filters filters, SortOptions sortOptions) where T : class
        {
            if (filters == null)
            {
                filters = new Filters();
            }

            if (sortOptions == null)
            {
                sortOptions = new SortOptions();
            }

            var result = filters.ToSqlAndValues();

            var items = this._db.Fetch<T>(result.Sql + sortOptions.ToSql(), result.Values);

            return items;
        }

        public T SingleOrDefault<T>(Filters filters) where T : class
        {
            var result = filters.ToSqlAndValues();

            var obj = this._db.SingleOrDefault<T>(result.Sql, result.Values);

            return obj;
        }

        public T FirstOrDefault<T>(Filters filters) where T : class
        {
            var result = filters.ToSqlAndValues();

            var obj = this._db.FirstOrDefault<T>(result.Sql, result.Values);
            
            return obj;
        }

        public int Update<T>(Fields fields, Filters filters) where T : class
        {
            if (filters == null)
            {
                filters = new Filters();
            }

            var result1 = fields.ToSqlAndValues();
            var result2 = filters.ToSqlAndValues(result1.Values.Length);

            var sql = result1.Sql + result2.Sql;
            var values = new object[result1.Values.Length + result2.Values.Length];
            result1.Values.CopyTo(values, 0);
            result2.Values.CopyTo(values, result1.Values.Length);

            return this._db.Update<T>(sql, values);
        }

        public int Update<T>(T entity) where T : class
        {
            return this._db.Update(entity);
        }


        private class CountList
        {

            public string Cnt { get; set; }

            public string Name { get; set; }

        }
    }
}