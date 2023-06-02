using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters;

public class FilterProvider
{
    private Dictionary<Type, Type> _filterTypes = new();

    public readonly Dictionary<string, Dictionary<string, Func<object, object>>> Mappers = new();
    public object MapValue(object value, string table, string propertyName)
    {
        if (!Mappers.TryGetValue(table, out var tabMappers)) return value;
        return tabMappers.TryGetValue(propertyName, out var func) ? func.Invoke(value) : value;
    }
    
    public void MapApiToContext(Type apiType, Type set)
    {
        _filterTypes.TryAdd(apiType, set);
    }
    
    public Type? GetDbTable(string tableName)
    {
        return _filterTypes.FirstOrDefault(pair => pair.Key.Name == tableName).Value;
    }


    public List<FilterInfo> GetFilters(string tableName)
    {
        var set = _filterTypes.FirstOrDefault(pair => pair.Key.Name == tableName).Value;

        if (set is null)
            throw new Exception("Table not mapped");

        var properties = set.GetProperties();
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