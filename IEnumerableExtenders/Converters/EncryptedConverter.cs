using Pandatech.Crypto;

namespace PandaTech.IEnumerableFilters.Converters;

internal class EncryptedConverter : IConverter<string, byte[]>
{
    public static Aes256 _aes256 = new(new()
    {
        Key = Environment.GetEnvironmentVariable("AES_KEY")
    });
    
    public byte[] ConvertTo(string from)
    {
        return _aes256.Encrypt(from);
    }

    public string ConvertFrom(byte[] to)
    {
        return _aes256.Decrypt(to)!;
    }
}