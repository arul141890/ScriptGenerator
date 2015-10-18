// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoManager.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The dto manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    public static class DtoManager
    {

        public static T DecryptAndGetDto<T>(string id, string key, string data) where T : class 
        {
            var plainText = data.Decrypt(key);
            return plainText.DeserializeFromJson<T>();
        }

        public static string EncryptAndGetDto(string id, string key, object obj)
        {
            var plainText = obj.SerializeToJson();
            var cipherText = plainText.Encrypt(key);
            return cipherText;
        }

    }
}