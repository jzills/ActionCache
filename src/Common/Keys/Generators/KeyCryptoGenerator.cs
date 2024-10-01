using System.Security.Cryptography;
using System.Text;

namespace ActionCache.Common.Keys;

internal class KeyCryptoGenerator
{
    internal protected readonly AESCrypto AES;

    // TODO: Use the route values/action args as input to the constructor
    // to generate deterministic encryption in order to regenerate keys
    // for cache retrieval, etc.
    internal KeyCryptoGenerator()
    {
        AES = new AESCrypto("ABC123");
    }   

    internal string Encrypt(string value) => 
        Convert.ToHexString(AES.Encrypt(Encoding.UTF8.GetBytes(value)));

    internal string Decrypt(string value) => 
        Encoding.UTF8.GetString(AES.Decrypt(Convert.FromHexString(value)));

    public class AESCrypto
    {
        private readonly byte[] Key;
        private readonly byte[] IV = new byte[16]; // Constant for now

        public AESCrypto(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        public byte[] Encrypt(byte[] value)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;

                // Use a hash of the input to deterministically
                // create an encrypted value that can be recreated
                // when fetching from the cache.
                byte[] IV = new byte[16];
                using (var sha256 = SHA256.Create())
                {
                    var ivHash = sha256.ComputeHash(value);
                    Array.Copy(ivHash, IV, 16);
                }

                using (var stream = new MemoryStream())
                {
                    stream.Write(IV, 0, IV.Length);

                    var encryptor = aes.CreateEncryptor(aes.Key, IV);
                    using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(value, 0, value.Length);
                    }

                    return stream.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] value)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;

                byte[] IV = new byte[16]; // 128-bit IV
                Array.Copy(value, 0, IV, 0, IV.Length);
               
                var decryptor = aes.CreateDecryptor(aes.Key, IV);
                using (MemoryStream stream = new MemoryStream(value, IV.Length, value.Length - IV.Length))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptedStream = new MemoryStream())
                        {
                            cryptoStream.CopyTo(decryptedStream);
                            return decryptedStream.ToArray(); 
                        }
                    }
                }
            }
        }
    }
}