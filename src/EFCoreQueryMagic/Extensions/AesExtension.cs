using EFCoreQueryMagic.Converters;
using Microsoft.AspNetCore.Builder;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Extensions;

public static class AesExtension
{
    public static WebApplicationBuilder ConfigureEncryptedConverter(this WebApplicationBuilder builder, string aesKey)
    {
        EncryptedConverter.Aes256 = new Aes256(new Aes256Options
        {
            Key = aesKey
        });

        return builder;
    }
}