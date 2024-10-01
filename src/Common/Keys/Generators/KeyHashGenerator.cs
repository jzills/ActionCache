using System.Security.Cryptography;
using System.Text;

namespace ActionCache.Common.Keys;

public static class KeyHashGenerator
{
    public static string ToHash(string value) =>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(value)));
}