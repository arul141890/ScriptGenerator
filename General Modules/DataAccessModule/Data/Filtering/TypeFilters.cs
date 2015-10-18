// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFilters.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Filtering
{
    using System;
    using System.Linq.Expressions;

    public class TypeFilters<T> : Filters
    {

        public TypeFilters<T> And(Expression<Func<T, object>> expression, ConditionType conditionType, object value)
        {
            this.And(expression.GetPropertyName(), conditionType, value);
            return this;
        }

        public TypeFilters<T> AndSql(Expression<Func<T, object>> expression, ConditionType conditionType, string value)
        {
            this.AndSql(expression.GetPropertyName(), conditionType, value);
            return this;
        }

        public TypeFilters<T> Or(Expression<Func<T, object>> expression, ConditionType conditionType, object value)
        {
            this.Or(expression.GetPropertyName(), conditionType, value);
            return this;
        }

    }
}