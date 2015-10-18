// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlHelper.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The url helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System;
    using System.Web;

    public static class UrlHelper
    {

        public static string AppendParameterAndGetUrl(this string url, string key, string value)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[key] = value;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public static string RemoveParameterAndGetUrl(this string url, string key)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Remove(key);
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public static string GetUrlWithoutQueryString(string url)
        {
            var uri = new Uri(url);
            var path = string.Format("{0}{1}{2}{3}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority, uri.AbsolutePath);

            return path;
        }

    }
}