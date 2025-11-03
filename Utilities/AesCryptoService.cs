using System.Security.Cryptography;
using System.Text;

namespace Portfolio_Api.Utilities
{
    public class AesCryptoService
    {
        private readonly string _key;

        public AesCryptoService()
        {
            _key = AppConfig.GetEncryptionKey(); // ✅ fetch directly from global config
        }

        public string Decrypt(string cipherTextBase64)
        {
            var fullCipher = Convert.FromBase64String(cipherTextBase64);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key);

            // first 16 bytes = IV
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(fullCipher, 16, fullCipher.Length - 16);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
