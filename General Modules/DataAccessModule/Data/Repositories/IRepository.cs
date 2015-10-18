// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The Repository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Repositories
{
    using System;
    using System.Collections.Generic;

    using DataAccessModule.Data.Filtering;
    using DataAccessModule.Data.Sorting;
    using DataAccessModule.Data.Tables;

    public interface IRepository
    {
        // Transaction Management

        void BatchInsert<T>(IList<T> entities) where T : class;

        // Select

        // Aggregates
        int Count<T>() where T : class;

        int Count<T>(Filters filters) where T : class;

        int Delete<T>(T entity) where T : class;

        int Delete<T>(Filters filters) where T : class;

        void ExecuteAtomic(Action action);

        int ExecuteNonQuery(string query);

        // Custom

        IList<T> ExecuteQuery<T>(string query, params object[] values) where T : class;

        T ExecuteScalar<T>(string query);

        void Insert<T>(T entity) where T : class;

        PagedList<T> PagedSelectAll<T>(int page, int pageSize, Filters filters, SortOptions sortOptions) where T : class;

        IList<T> SelectAll<T>(Filters filters, SortOptions sortOptions) where T : class;

        T SingleOrDefault<T>(Filters filters) where T : class;

        T FirstOrDefault<T>(Filters filters) where T : class;

        int Update<T>(Fields fields, Filters filters) where T : class;

        int Update<T>(T entity) where T : class;

    }
}