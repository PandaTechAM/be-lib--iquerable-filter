using TableFilteringHelpers.Dto;

namespace TableFilteringHelpers;

public class FilterProvider
{
    private Dictionary<Type, Type> _filterTypes = new();

    public void MapApiToContext(Type apiType, Type set)
    {
        _filterTypes.TryAdd(apiType, set);
    }

    public List<FilterInfo> GetFilters(string tableName)
    {
        var set = _filterTypes.FirstOrDefault(pair => pair.Key.Name == tableName).Value;

        if (set is null)
            throw new Exception("Table not mapped");

        var properties = set!.GetProperties();
        var filterInfos = new List<FilterInfo>();
        foreach (var property in properties)
        {
            if (!EnumerableExtenders.ComparisonTypes.TryGetValue(property.PropertyType.Name, out var comparisonTypes))
                continue;
            var filterInfo = new FilterInfo
            {
                PropertyName = property.Name,
                ComparisonTypes = comparisonTypes,
                Table = tableName
            };
            filterInfos.Add(filterInfo);
        }

        return filterInfos;
    }

    public List<string> GetTables()
    {
        return _filterTypes.Keys.Select(type => type.Name).ToList();
    }
}