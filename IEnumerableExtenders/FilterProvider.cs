using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters;

public class FilterProvider
{
    public readonly List<Filter> Filters = new();
    public static List<object> Parameters { get; set; } = new();

    public class Filter
    {
        public string PropertyName { get; set; } = null!;
        public string TableName { get; set; } = null!;

        public List<ComparisonType> ComparisonTypes { get; set; } = null!;
        public Type TargetPropertyType { get; set; } = null!;
        public Type FilterType { get; set; } = null!;

        public Func<object, object> Converter { get; set; } = null!;
        public Expression? SourcePropertyConverter { get; set; }

        public Dictionary<ComparisonType, Func<object, bool>> Comparisons { get; set; } = new();
    }

    public void AddFilter(Filter filter)
    {
        Filters.Where(f =>
                f.TableName == filter.TableName && f.PropertyName == filter.PropertyName)
            .ToList()
            .ForEach(f => Filters.Remove(f));


        Filters.Add(filter);
    }


    public void AddFilter<TDto, TDb>()
    {
        var dtoType = typeof(TDto);
        var dbType = typeof(TDb);

        foreach (var dtoProperty in dtoType.GetProperties())
        {
            var dbProperty = dbType.GetProperty(dtoProperty.Name);

            if (dbProperty == null) continue;

            if (dbProperty.PropertyType != dtoProperty.PropertyType) continue;

            var comparisonTypes = dtoProperty.PropertyType.IsEnum
                ? EnumerableExtenders.ComparisonTypes["Enum"]
                : EnumerableExtenders.ComparisonTypes[dtoProperty.PropertyType.Name];

            var filter = new Filter
            {
                PropertyName = dtoProperty.Name,
                ComparisonTypes = comparisonTypes,
                Converter = value => value,
                SourcePropertyConverter = null,
                TableName = dtoType.Name,
                TargetPropertyType = dbProperty.PropertyType,
                FilterType = dbProperty.PropertyType
            };

            Filters.Add(filter);
        }
    }

    public List<FilterInfo> GetFilters(string tableName)
    {
        return Filters.Where(f => f.TableName == tableName)
            .Select(f => new FilterInfo
            {
                PropertyName = f.PropertyName,
                ComparisonTypes = f.ComparisonTypes,
                Table = f.TableName
            }).ToList();

        /*var dtoType = GetDtoType(tableName);
        var properties = dtoType.GetProperties();

        foreach (var property in properties)
        {
            if (filters.Any(f => f.PropertyName == property.Name)) continue;

            var dbType = GetDbTable(tableName);
            var dbProperties = dbType.GetProperties();
            var dbProperty = dbProperties.FirstOrDefault(p => p.Name == property.Name);
            if (dbProperty == null) continue;
            if (dbProperty.PropertyType != property.PropertyType) continue;

            filters.Add(new FilterInfo
            {
                PropertyName = property.Name,
                ComparisonTypes = IEnumerableFilters.EnumerableExtenders.ComparisonTypes[property.PropertyType.Name],
                Table = tableName
            });
        }


        return filters;*/
    }

    public List<string> GetTables()
    {
        return Filters.Select(f => f.GetType().GenericTypeArguments[0].Name).Distinct().ToList();
    }


    public Type GetDbTable(string tableName)
    {
        return Filters.Where(f => f.GetType().GenericTypeArguments[0].Name == tableName)
            .Select(f => f.GetType().GenericTypeArguments[1]).First();
    }

    public Type GetDtoType(string tableType)
    {
        return Filters.Where(f => f.GetType().GenericTypeArguments[0].Name == tableType)
            .Select(f => f.GetType().GenericTypeArguments[0]).First();
    }

    public Type GetDtoType(Type tableType)
    {
        return Filters.Where(f => f.GetType().GenericTypeArguments[1] == tableType)
            .Select(f => f.GetType().GenericTypeArguments[0]).First();
    }

    public Filter? GetFilter(string filterDtoPropertyName, ComparisonType filterDtoComparisonType)
    {
        return Filters.FirstOrDefault(f =>
            f.PropertyName == filterDtoPropertyName && f.ComparisonTypes.Contains(filterDtoComparisonType));
    }

    public Filter GetFilter<TSource, TFilterType, TResultType>(
        string filterDtoPropertyName, ComparisonType filterDtoComparisonType)
    {
        return (Filters.First(f =>
            f.PropertyName == filterDtoPropertyName && f.ComparisonTypes.Contains(filterDtoComparisonType)));
    }
}