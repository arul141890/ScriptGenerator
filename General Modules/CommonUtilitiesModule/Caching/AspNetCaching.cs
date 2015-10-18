// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetCaching.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The asp net caching.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.Caching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web;

    public class AspNetCaching : CacheProvider
    {

        public override object Get(string group, string key)
        {
            var obj = HttpContext.Current.Cache[this.MakeKeyFromGroupAndKey(group, key)];

            return obj;
        }

        public override IList<CacheItem> GetCachedItems()
        {
            var lst = new List<CacheItem>();

            foreach (DictionaryEntry entry in HttpContext.Current.Cache)
            {
                var key = entry.Key.ToString();

                if (this.IsPossibleKey(key))
                {
                    var gk = this.GetGroupAndKeyFromKey(key);
                    if (gk.Length == 2)
                    {
                        var item = new CacheItem {Group = gk[0], Key = gk[1], Value = entry.Value};

                        lst.Add(item);
                    }
                }
            }

            return lst;
        }

        public override void Invalidate(string group, string key)
        {
            HttpContext.Current.Cache.Remove(this.MakeKeyFromGroupAndKey(group, key));
        }

        public override void Put(string group, string key, object obj, DateTime expiry)
        {
            HttpContext.Current.Cache.Insert(this.MakeKeyFromGroupAndKey(group, key), obj, null, expiry, TimeSpan.Zero);
        }

    }
}