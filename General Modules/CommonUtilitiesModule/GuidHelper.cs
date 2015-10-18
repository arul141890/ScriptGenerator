namespace CommonUtilitiesModule
{
    using System;

    public static class GuidHelper
    {
        public static string NewShortGuid()
        {
            var guid = Guid.NewGuid();
            string encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded
                .Replace("/", "")
                .Replace("+", "");
            return (encoded.Length > 22) ? encoded.Substring(0, 22) : encoded;
        }
    }
}