using EFCoreQueryMagic.Converters;
using Microsoft.AspNetCore.Builder;

namespace EFCoreQueryMagic.Extensions;

public static class AesExtension
{
    public static WebApplicationBuilder ConfigureEncryptedConverter(this WebApplicationBuilder builder,  string aesKey)
    {
        EncryptedConverter.Aes256 = new(new()
        {
            Key = aesKey
        });
        
        return builder;
    }
    
}