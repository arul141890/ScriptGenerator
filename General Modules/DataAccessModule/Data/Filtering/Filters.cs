// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Filters.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The filters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Filtering
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public class Filters
    {

        private readonly IList<Filter> filters = new List<Filter>();

        public int Count
        {
            get
            {
                return this.filters.Count;
            }
        }

        public Dictionary<string,string> ToKeyValueDictionary()
        {
            var dic = new Dictionary<string, string>();

            foreach (var filter in filters)
            {
                dic.Add(filter.Name, filter.Value.ToString());
            }

            return dic;
        } 



        public Filters And(string name, ConditionType conditionType, object value)
        {
            this.filters.Add(
                new Filter { Name = name, Value = value, FilterType = FilterType.And, ConditionType = conditionType });

            return this;
        }


        public Filters AndSql(string name, ConditionType conditionType, string value)
        {
            this.filters.Add(
                new Filter
                    {
                        Name = name,
                        Value = value,
                        FilterType = FilterType.And,
                        ValueType = ValueType.Sql,
                        ConditionType = conditionType
                    });

            return this;
        }

        public object GetValue(string name)
        {
            if (name != null)
            {
                var y = this.filters.SingleOrDefault(x => x.Name.ToLower() == name.ToLower());

                if (y != null)
                {
                    return y.Value;
                }
            }

            return null;
        }

        public Filters Or(string name, ConditionType conditionType, object value)
        {
            this.filters.Add(
                new Filter { Name = name, Value = value, FilterType = FilterType.Or, ConditionType = conditionType });

            return this;
        }

        public SqlAndValues ToSqlAndValues(int seed = 0)
        {
            var sb = new StringBuilder();
            var x = 0;

            var y = 0;
            foreach (var filter in this.filters)
            {
                if (filter.Value is IList)
                {
                    x += (filter.Value as IList).Count;
                    y++;
                }
            }

            var size = this.filters.Count + ((x > 0) ? x - y : x);

            var values = new object[size];

            if (this.filters.Count > 0)
            {
                sb.Append(" WHERE ");
            }

            var i = seed;
            foreach (var filter in this.filters)
            {
                if (i - seed > 0)
                {
                    sb.AppendFormat(" {0} ", (filter.FilterType == FilterType.And) ? "AND" : "OR");
                }

                var setValue = true;

                switch (filter.ConditionType)
                {
                    case ConditionType.EqualTo:
                        sb.AppendFormat(filter.Value == null ? " {0} IS @{1}" : " {0}=@{1}", filter.Name, i);
                        break;
                    case ConditionType.NotEqualTo:
                        sb.AppendFormat(filter.Value == null ? " {0} IS NOT @{1}" : " {0}<>@{1}", filter.Name, i);
                        break;
                    case ConditionType.LessThan:
                        sb.AppendFormat(" {0}<@{1}", filter.Name, i);
                        break;
                    case ConditionType.GreaterThan:
                        sb.AppendFormat(" {0}>@{1}", filter.Name, i);
                        break;
                    case ConditionType.Like:
                        sb.AppendFormat(" {0} LIKE @{1}", filter.Name, i);
                        break;
                    case ConditionType.NotLike:
                        sb.AppendFormat(" {0} NOT LIKE @{1}", filter.Name, i);
                        break;
                    case ConditionType.LessThanOrEqualTo:
                        sb.AppendFormat(" {0}<=@{1}", filter.Name, i);
                        break;
                    case ConditionType.GreaterThanOrEqualTo:
                        sb.AppendFormat(" {0}>=@{1}", filter.Name, i);
                        break;
                    case ConditionType.In:

                        var lst = filter.Value as IList;
                        if (null == lst || lst.Count == 0)
                        {
                            throw new Exception("For IN condition, filter value MUST be a list.");
                        }

                        sb.AppendFormat(" {0} in (", filter.Name);

                        var first = true;
                        foreach (var value in lst)
                        {
                            if (!first)
                            {
                                sb.Append(",");
                            }

                            sb.AppendFormat("@{0}", i);

                            values[i - seed] = value;

                            if (first)
                            {
                                first = false;
                            }

                            i++;
                        }

                        sb.Append(")");

                        setValue = false;
                        break;
                    default:
                        throw new Exception("Unsupported condition");
                }

                if (setValue)
                {
                    values[i - seed] = filter.Value;
                    i++;
                }
            }

            return new SqlAndValues { Sql = sb.ToString(), Values = values };
        }


        private class Filter
        {

            public ConditionType ConditionType { get; set; }

            public FilterType FilterType { get; set; }

            public string Name { get; set; }

            public object Value { get; set; }

            public ValueType ValueType { get; set; }

        }
    }
}