// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheProvider.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The CacheProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.Caching
{
    using System;
    using System.Collections.Generic;

    public interface ICacheProvider
    {

        object Get(string group, string key);

        object Get(string group, string key, bool alwaysTakeFromSource, DateTime expiry, Func<object> func);

        IList<CacheItem> GetCachedItems();

        void Invalidate(string group, string key);

        void Put(string group, string key, object obj, DateTime expiry);

    }
}