// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortOptions.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The sort options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Sorting
{
    using System.Collections.Generic;
    using System.Text;

    public class SortOptions
    {

        private readonly IList<SortOption> options = new List<SortOption>();



        public SortOptions Add(string name, bool descending)
        {
            this.options.Add(new SortOption { Name = name, Descending = descending });

            return this;
        }

        public string ToSql()
        {
            var sb = new StringBuilder();

            if (this.options.Count > 0)
            {
                sb.Append(" ORDER BY ");
            }

            var first = true;
            foreach (var option in this.options)
            {
                sb.AppendFormat(
                    "{0} {1} {2}", first ? string.Empty : ",", option.Name, option.Descending ? "DESC" : "ASC");
                first = false;
            }

            return sb.ToString();
        }


        private class SortOption
        {

            public bool Descending { get; set; }

            public string Name { get; set; }

        }
    }
}