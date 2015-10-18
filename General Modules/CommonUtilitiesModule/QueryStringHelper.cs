// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryStringHelper.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System.Collections.Specialized;
    using System.Text;
    using System.Web;

    public class QueryString : NameValueCollection
    {

        public QueryString()
        {
        }

        public QueryString(string queryString)
        {
            this.FillFromString(queryString);
        }



        public static QueryString Current
        {
            get { return new QueryString().FromCurrent(); }
        }



        public new string this[string name]
        {
            get { return HttpUtility.UrlDecode(base[name]); }
        }

        public new string this[int index]
        {
            get { return HttpUtility.UrlDecode(base[index]); }
        }



        public new QueryString Add(string name, string value)
        {
            return this.Add(name, value, false);
        }

        public QueryString Add(string name, string value, bool isUnique)
        {
            var existingValue = base[name];
            if (string.IsNullOrEmpty(existingValue))
            {
                base.Add(name, HttpUtility.UrlEncodeUnicode(value));
            }
            else if (isUnique)
            {
                base[name] = HttpUtility.UrlEncodeUnicode(value);
            }
            else
            {
                base[name] += "," + HttpUtility.UrlEncodeUnicode(value);
            }

            return this;
        }

        public bool Contains(string name)
        {
            var existingValue = base[name];
            return !string.IsNullOrEmpty(existingValue);
        }

        public string ExtractQuerystring(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Contains("?"))
                {
                    return s.Substring(s.IndexOf("?") + 1);
                }
            }

            return s;
        }

        public QueryString FillFromString(string s)
        {
            this.Clear();
            if (string.IsNullOrEmpty(s))
            {
                return this;
            }

            foreach (var keyValuePair in this.ExtractQuerystring(s).Split('&'))
            {
                if (string.IsNullOrEmpty(keyValuePair))
                {
                    continue;
                }

                var split = keyValuePair.Split('=');
                base.Add(split[0], split.Length == 2 ? split[1] : string.Empty);
            }

            return this;
        }

        public QueryString FromCurrent()
        {
            if (HttpContext.Current != null)
            {
                return this.FillFromString(HttpContext.Current.Request.QueryString.ToString());
            }

            this.Clear();
            return this;
        }

        public new QueryString Remove(string name)
        {
            var existingValue = base[name];
            if (!string.IsNullOrEmpty(existingValue))
            {
                base.Remove(name);
            }

            return this;
        }

        public QueryString Reset()
        {
            this.Clear();
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < base.Keys.Count; i++)
            {
                if (!string.IsNullOrEmpty(base.Keys[i]))
                {
                    foreach (var val in base[base.Keys[i]].Split(','))
                    {
                        builder.Append((builder.Length == 0) ? "?" : "&").Append(
                            HttpUtility.UrlEncodeUnicode(base.Keys[i])).Append("=").Append(val);
                    }
                }
            }

            return builder.ToString();
        }

    }
}