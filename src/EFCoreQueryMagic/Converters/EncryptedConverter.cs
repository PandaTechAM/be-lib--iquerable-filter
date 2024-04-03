using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Converters;

public class EncryptedConverter : IConverter<string?, byte[]?>
{
    public static Aes256 Aes256 { get; set; } = null!;

    public DbContext Context { get; set; } = null!;

    public byte[]? ConvertTo(string? from)
    {
        return from is null ? null : Aes256.Encrypt(from);
    }

    public string? ConvertFrom(byte[]? to)
    {
        return to is null ? null : Aes256.Decrypt(to)!;
    }
}