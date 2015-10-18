// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheItem.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The cache item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.Caching
{
    public class CacheItem
    {

        public string Group { get; set; }

        public string Key { get; set; }

        public object Value { get; set; }

    }
}