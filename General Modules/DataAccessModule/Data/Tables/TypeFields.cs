// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFields.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The type fields.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Tables
{
    using System;
    using System.Linq.Expressions;

    using DataAccessModule.Data.Filtering;

    public class TypeFields<T> : Fields
    {
        public TypeFields<T> Add(Expression<Func<T, object>> expression, object value)
        {
            this.Add(expression.GetPropertyName(), value);
            return this;
        }
    }
}