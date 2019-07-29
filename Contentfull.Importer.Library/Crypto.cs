using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Contentful.Importer.Library
{
    public class Crypto
    {
        public class Hash
        {
            public static string SHA256(string password)
            {
                SHA256Cng crypt = new SHA256Cng(); //Using SHA256ng for FIPS compliance
                
                string hash = String.Empty;
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
                foreach (byte bit in crypto)
                {
                    hash += bit.ToString("x2");
                }
                return hash;
            }
        }
        public class SymmetricEncryption
        {
            /// <summary>
            /// Rijndael Symmetrical Algorithm 
            /// </summary>
            public class FIPSAes
            {

                /// <summary>
                /// Encrypt a byte array into a byte array using a key and an IV 
                /// </summary>
                /// <param name="clearData"></param>
                /// <param name="Key"></param>
                /// <param name="IV"></param>
                /// <returns></returns>
                public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
                {
                    // Create a MemoryStream to accept the encrypted bytes 
                    using (MemoryStream ms = new MemoryStream())
                    {
                        AesCryptoServiceProvider alg = new AesCryptoServiceProvider();
                        //AesCryptoServiceProvider alg = AesCryptoServiceProvider.Create();
                        alg.Key = Key;
                        alg.IV = IV;
                        CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
                        cs.Write(clearData, 0, clearData.Length);
                        cs.Close();
                        byte[] encryptedData = ms.ToArray();
                        return encryptedData;
                    }
                }
                /// <summary>
                /// Encrypt a string into a string using a password 
                /// </summary>
                /// <param name="clearText"></param>
                /// <param name="Password"></param>
                /// <returns></returns>
                public static string Encrypt(string clearText, string Password)
                {

                    // First we need to turn the input string into a byte array. 
                    byte[] clearBytes = System.Text.Encoding.UTF8.GetBytes(clearText);
                    PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                        new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
                0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                    byte[] encryptedData = Encrypt(clearBytes,
                             pdb.GetBytes(32), pdb.GetBytes(16));
                    return Convert.ToBase64String(encryptedData);

                }
                /// <summary>
                /// Decrypt a byte array into a byte array using a key and an IV 
                /// </summary>
                /// <param name="cipherData"></param>
                /// <param name="Key"></param>
                /// <param name="IV"></param>
                /// <returns></returns> 
                public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
                {
                    // Create a MemoryStream that is going to accept the
                    // decrypted bytes 
                    using (MemoryStream ms = new MemoryStream())
                    {
                        AesCryptoServiceProvider alg = new AesCryptoServiceProvider();
                        alg.Key = Key;
                        alg.IV = IV;
                        CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                        cs.Write(cipherData, 0, cipherData.Length);
                        cs.Close();
                        byte[] decryptedData = ms.ToArray();
                        return decryptedData;
                    }
                }
                /// <summary>
                /// Decrypt a string into a string using a password 
                /// </summary>
                /// <param name="cipherText"></param>
                /// <param name="Password"></param>
                /// <returns></returns>
                public static string Decrypt(string cipherText, string Password)
                {

                    // First we need to turn the input string into a byte array. 
                    // We presume that Base64 encoding was used 
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                        new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65,
                0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                    byte[] decryptedData = Decrypt(cipherBytes,
                        pdb.GetBytes(32), pdb.GetBytes(16));
                    return System.Text.Encoding.UTF8.GetString(decryptedData);
                }
            }
        }
      
    }
}
