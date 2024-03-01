using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;

namespace PandaTech.IEnumerableFilters.Converters;

public class EncryptedConverter : IConverter<string, byte[]>
{
    public static Aes256 Aes256 = new(new()
    {
        Key = Environment.GetEnvironmentVariable("AES_KEY") ?? ""
    });

    public DbContext Context { get; set; }

    public byte[] ConvertTo(string from)
    {
        return Aes256.Encrypt(from);
    }

    public string ConvertFrom(byte[] to)
    {
        return Aes256.Decrypt(to)!;
    }
}