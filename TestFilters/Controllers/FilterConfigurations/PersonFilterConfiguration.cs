/*using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.v4;
using TestFilters.Controllers.Models;

namespace TestFilters.Controllers.FilterConfigurations;

public class PersonFilterConfiguration: FilterConfigurator<Person>
{
    public PersonFilterConfiguration()
    {
        Filter(x => x.Name)
            .Comparison(ComparisonType.Contains).ConvertValues<long, string>(x => x.ToString()).Lambda("Name.Contains(@0)");
    }
}*/