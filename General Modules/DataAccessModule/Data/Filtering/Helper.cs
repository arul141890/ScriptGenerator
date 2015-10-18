// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helper.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Filtering
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class Helper
    {
        public static string  GetPropertyName<T>(this Expression<Func<T, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    return propertyInfo.Name;
                }
            }

            return null;
        }
    }
}