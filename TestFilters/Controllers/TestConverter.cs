using PandaTech.IEnumerableFilters;

namespace TestFilters.Controllers;

public class TestConverter: IConverter<string, long>
{
    public string Convert(long storeValue) => "_" + storeValue;

    public long Convert(string value) => long.Parse(value[1..]);
}