// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Encryptor.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The encryptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class Encryptor
    {

        public static string Decrypt(this string cipherText, string encryptionKey)
        {
            // if(cipherText.Contains(" "))
            cipherText = cipherText.Replace(" ", "+");

            byte[] resultArray;

            var toEncryptArray = Convert.FromBase64String(cipherText);

            byte[] keyArray;

            // If hashing was used get the hash code with regards to your key
            using (var hashmd5 = new MD5CryptoServiceProvider())
            {
                keyArray = hashmd5.ComputeHash(Encoding.ASCII.GetBytes(encryptionKey));
                hashmd5.Clear();
            }

            // Set the secret key for the tripleDES algorithm
            using (
                var tdes = new TripleDESCryptoServiceProvider
                {
                    Key = keyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })
            {
                var cTransform = tdes.CreateDecryptor();
                resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
            }

            // Return the Clear decrypted TEXT
            return Encoding.ASCII.GetString(resultArray);
        }

        public static string Encrypt(this string plainText, string encryptionKey)
        {
            var toEncryptArray = Encoding.ASCII.GetBytes(plainText);

            byte[] encryptionKeyArray;

            // get MD5 has for the key
            using (var hashmd5 = new MD5CryptoServiceProvider())
            {
                encryptionKeyArray = hashmd5.ComputeHash(Encoding.ASCII.GetBytes(encryptionKey));
                hashmd5.Clear();
            }

            byte[] resultArray;

            // Set the secret key for the tripleDES algorithm
            using (
                var tdes = new TripleDESCryptoServiceProvider
                {
                    Key = encryptionKeyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })
            {
                // Transform the specified region of bytes array to resultArray
                var cTransform = tdes.CreateEncryptor();
                resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
            }

            // Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray);
        }

    }
}