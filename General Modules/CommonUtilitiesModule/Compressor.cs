// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Compressor.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The compressor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    public static class Compressor
    {

        public static byte[] CompressToGzip(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        msi.CopyTo(gs);
                    }

                    return mso.ToArray();
                }
            }
        }

        public static string DecompressFromGzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        gs.CopyTo(mso);
                    }

                    return Encoding.Default.GetString(mso.ToArray());
                }
            }
        }

    }
}