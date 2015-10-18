// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public static class Extensions
    {

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static bool CaseInsensitiveEquals(this string a, string b)
        {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static Expression<T> Compose<T>(
            this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new {f, s = second.Parameters[i]}).ToDictionary(
                p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static string GetPostData(HttpRequest request)
        {
            var myStream = request.InputStream;

            // Read the file into the byte array.
            var input = new byte[request.InputStream.Length];
            myStream.Read(input, 0, (int) request.InputStream.Length - 1);

            // encode to a string
            var enc = new UTF8Encoding();
            var sPostData = enc.GetString(input);

            return sPostData;
        }

        public static string GetWordsFromCamelCase(this string value)
        {
            if (value.EndsWith("Id"))
            {
                return value;
            }

            var sb = new StringBuilder();

            foreach (var ch in value)
            {
                if (char.IsUpper(ch))
                {
                    sb.Append(" ");
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                default:
                    return false;
            }
        }

        public static string MaxString(string s, int length)
        {
            if (s == null)
            {
                return null;
            }

            if (s.Length > length)
            {
                return s.Substring(0, length);
            }

            return s;
        }

        public static string Md5(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            using (var hashmd5 = new MD5CryptoServiceProvider())
            {
                var bytes = hashmd5.ComputeHash(Encoding.Default.GetBytes(text));
                hashmd5.Clear();

                var sb = new StringBuilder();
                for (var i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string MimeType(this string extension)
        {
            return MimeHelper.GetMimeMapping(extension);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        public static string ReplaceFirstOccurrance(this string original, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(original))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(oldValue))
            {
                return original;
            }

            if (string.IsNullOrEmpty(newValue))
            {
                newValue = string.Empty;
            }

            var loc = original.IndexOf(oldValue, StringComparison.Ordinal);
            return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }

        public static T StringToEnum<T>(this string s)
        {
            return (T) Enum.Parse(typeof (T), s, true);
        }

        public static List<int> ToIntList(this string s, string delimiters)
        {
            var a = s.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return a.ToList().ConvertAll(Convert.ToInt32).ToList();
        }

        public static string ToQueryString(this NameValueCollection nvc)
        {
            return string.Join(
                "&",
                Array.ConvertAll(
                    nvc.AllKeys,
                    key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
        }

    }
}