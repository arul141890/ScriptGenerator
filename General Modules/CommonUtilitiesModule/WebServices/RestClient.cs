// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestClient.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The rest client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule.WebServices
{
    using System;
    using System.IO;
    using System.Net;

    public static class RestClient
    {

        public static T GetJson<T>(string url) where T : class 
        {
            return PostAndGetJson<T>(url, null);
        }

        public static T PostAndGetJson<T>(string url, object obj) where T : class 
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            try
            {
                var req = (HttpWebRequest) WebRequest.Create(url);

                req.ContentType = "application/json";

                using (var s = req.GetRequestStream())
                {
                    using (var sw = new StreamWriter(s)) sw.Write(obj.SerializeToJson());
                }

                req.Accept = "application/json";

                var res = req.GetResponse();

                using (var stream = res.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        return json.DeserializeFromJson<T>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}