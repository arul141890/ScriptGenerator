// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Serializer.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The serializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;

    public static class Serializer
    {

        public static T DeserializeFromJson<T>(this string json) where T : class 
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof (T));
                return (T) serializer.ReadObject(ms);
            }
        }

        public static T DeserializeFromNewtonSoftJson<T>(this string json) where T : class 
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T DeserializeFromXml<T>(this string xml) where T : class 
        {
            var obj = Activator.CreateInstance<T>();

            using (var ms = new MemoryStream(Encoding.Default.GetBytes(xml)))
            {
                var serializer = new XmlSerializer(obj.GetType());
                obj = (T) serializer.Deserialize(ms);
            }

            return obj;
        }

        public static string SerializeToJson<T>(this T obj) where T : class 
        {
            if (null == obj)
            {
                return null;
            }

            var serializer = new DataContractJsonSerializer(obj.GetType());

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                var retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }
        }

        public static string SerializeToNewtonSoftJson<T>(this T obj) where T : class 
        {
            if (null == obj)
            {
                return null;
            }

            return JsonConvert.SerializeObject(obj);
        }

        public static string SerializeToXml<T>(this T obj) where T : class 
        {
            if (null == obj)
            {
                return null;
            }

            var serializer = new XmlSerializer(obj.GetType());

            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                var retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }
        }

    }
}