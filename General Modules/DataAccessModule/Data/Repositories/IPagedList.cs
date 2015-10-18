// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPagedList.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The paged list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Repositories
{
    using System.Collections.Generic;

    public class PagedList<T> : List<T>
    {

        public long CurrentPage { get; set; }

        public long PageSize { get; set; }

        public long TotalItems { get; set; }

        public long TotalPages { get; set; }

    }
}