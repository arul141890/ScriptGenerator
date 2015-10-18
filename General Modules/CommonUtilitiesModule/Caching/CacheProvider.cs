// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheProvider.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The cache value wrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.Caching
{
    using System;
    using System.Collections.Generic;

    public class CacheValueWrapper
    {

        public object Value { get; set; }

    }

    public abstract class CacheProvider : ICacheProvider
    {

        private const string Delimiter = "<::>";



        public object Get(string group, string key, bool alwaysTakeFromSource, DateTime expiry, Func<object> func)
        {
            var obj = alwaysTakeFromSource ? func() : this.Get(group, key);
            if (!alwaysTakeFromSource && null == obj)
            {
                obj = func();
                if (obj != null)
                {
                    this.Put(group, key, obj, expiry);
                }
            }

            return obj;
        }

        public abstract object Get(string group, string key);

        public abstract IList<CacheItem> GetCachedItems();

        public abstract void Invalidate(string group, string key);

        public abstract void Put(string group, string key, object obj, DateTime expiry);



        protected string[] GetGroupAndKeyFromKey(string key)
        {
            return key.Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries);
        }

        protected bool IsPossibleKey(string key)
        {
            return key != null && key.IndexOf(Delimiter, StringComparison.Ordinal) > 0;
        }

        protected string MakeKeyFromGroupAndKey(string group, string key)
        {
            return group + Delimiter + key;
        }

    }
}