using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters;

public class FilterProvider
{
    public readonly List<IFilter> Filters = new();

    public interface IFilter
    {
        public string TableName { get; set; }
        public Expression? SourcePropertyConverter { get; set; }
        public Func<object, object> Converter { get; set; }
        public string PropertyName { get; set; }
        public List<ComparisonType> ComparisonTypes { get; set; }

        public Type SourcePropertyType { get; set; }
        public Type TargetPropertyType { get; set; }

        public Type FilterType { get; set; }
    }

    public class Filter<TSource, TFilterType, TResultType> : IFilter
    {
        public string PropertyName { get; set; } = null!;
        public List<ComparisonType> ComparisonTypes { get; set; }
        public Type SourcePropertyType { get; set; }
        public Type TargetPropertyType { get; set; }
        public Type FilterType { get; set; }


        public Func<object, object> Converter { get; set; } = null!;
        public string TableName { get; set; }
        public Expression? SourcePropertyConverter { get; set; } = null!;

        public Filter()
        {
            SourcePropertyType = typeof(TSource);
            TargetPropertyType = typeof(TResultType);
            FilterType = typeof(TFilterType);
        }
    }

    public void AddFilter<TSource, TFilterType, TResultType>(Filter<TSource, TFilterType, TResultType> filter)
    {
        Filters.Where(f => f.TableName == filter.TableName && f.PropertyName == filter.PropertyName)
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
            
            var fType = typeof(Filter<,,>).MakeGenericType(dtoType, dbProperty.PropertyType, dbProperty.PropertyType);
            
            var filter = (IFilter) Activator.CreateInstance(fType)!;
            
            filter.PropertyName = dtoProperty.Name;
            filter.ComparisonTypes = EnumerableExtenders.ComparisonTypes[dtoProperty.PropertyType.Name];
            filter.Converter = value => value;
            filter.SourcePropertyConverter = null;
            filter.TableName = nameof(dtoType);
            filter.SourcePropertyType = dbType;
            filter.TargetPropertyType = dbProperty.PropertyType;
            filter.FilterType = dbProperty.PropertyType;
            
            Filters.Add(filter);

        }
        
        
    }

    public List<FilterInfo> GetFilters(string tableName)
    {
        var filters = new List<FilterInfo>();
        foreach (var filter in Filters)
        {
            var tName = filter.GetType().GenericTypeArguments[0].Name;

            if (tName != tableName) continue;

            if (Filters.Any(f => f.TableName == tName && f.PropertyName == filter.PropertyName))
            {
                filters.First(f => f.Table == tName && f.PropertyName == filter.PropertyName).ComparisonTypes
                    = filter.ComparisonTypes;
                continue;
            }
        }

        var dtoType = GetDtoType(tableName);
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


        return filters;
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

    public IFilter? GetFilter(string filterDtoPropertyName, ComparisonType filterDtoComparisonType)
    {
        return Filters.FirstOrDefault(f =>
            f.PropertyName == filterDtoPropertyName && f.ComparisonTypes.Contains(filterDtoComparisonType));
    }

    public Filter<TSource, TFilterType, TResultType> GetFilter<TSource, TFilterType, TResultType>(
        string filterDtoPropertyName, ComparisonType filterDtoComparisonType)
    {
        return (Filters.First(f =>
                f.PropertyName == filterDtoPropertyName && f.ComparisonTypes.Contains(filterDtoComparisonType)) as
            Filter<TSource, TFilterType, TResultType>)!;
    }
}