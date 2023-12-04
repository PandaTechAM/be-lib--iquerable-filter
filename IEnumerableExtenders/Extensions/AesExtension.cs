using Microsoft.AspNetCore.Builder;
using PandaTech.IEnumerableFilters.Converters;

namespace PandaTech.IEnumerableFilters.Extensions;

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