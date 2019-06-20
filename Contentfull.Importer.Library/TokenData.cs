using Newtonsoft.Json;
using System;

namespace Contentful.Importer.Library
{
    public class TokenData
    {
        public static string filename = "TokenData.dat";
        private static string Key { get { return "TokenData"; } }
        private static string EncKey { get { return "c0ntent" + PCInfo.GetMotherBoardID(); } }
        public string Token { get; set; }
        public string Username { get; set; }
        public string SpaceID { get; set; }
        public string Password { get; set; }
        public DateTime CreatedOn { get; set; }
        public TokenData()
        {
            CreatedOn = DateTime.Now;
        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this);
            var encryptionKey = EncKey;
            encryptionKey = Crypto.Hash.SHA256(encryptionKey);
            data = Crypto.SymmetricEncryption.RijndaelAlgorithm.Encrypt(data, encryptionKey);
            System.IO.File.WriteAllText(filename, data);

        }

        public static TokenData Load()
        {
            if (System.IO.File.Exists(filename))
            {
                var data = System.IO.File.ReadAllText(filename);
                var encryptionKey = EncKey;
                encryptionKey = Crypto.Hash.SHA256(encryptionKey);

                if (data.Length > 0)
                {

                    try
                    {
                        data = Crypto.SymmetricEncryption.RijndaelAlgorithm.Decrypt(data, encryptionKey);
                        return JsonConvert.DeserializeObject<TokenData>(data);

                    }
                    catch
                    {
                        throw new Exception("Failed decrypting token : Config was possibly moved from another PC");
                    }

                }
            }
            return new TokenData();
        }
    }
}
