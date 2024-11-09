using System.Security.Cryptography;
using System.Text;

namespace ActionCache.Common.Keys;

public class KeyEncoder
{
    public string Encode(string value) => 
        Convert.ToHexString(Encoding.UTF8.GetBytes(value));

    public string Decode(string value) => 
        Encoding.UTF8.GetString(Convert.FromHexString(value));
}