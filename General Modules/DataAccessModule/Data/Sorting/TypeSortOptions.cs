// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeSortOptions.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The type sort options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Sorting
{
    using System;
    using System.Linq.Expressions;

    using DataAccessModule.Data.Filtering;

    public class TypeSortOptions<T> : SortOptions
    {
        public TypeSortOptions<T> Add(Expression<Func<T,object>> expression, bool descending)
        {
            this.Add(expression.GetPropertyName(), descending);
            return this;
        }
    }
}