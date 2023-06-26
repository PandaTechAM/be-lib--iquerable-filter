using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters;

public class FilterProvider
{
    public readonly List<Filter> Filters = new();

    public class Filter
    {
        public string PropertyName { get; set; } = null!;
        public string TableName { get; set; } = null!;

        public List<ComparisonType> ComparisonTypes { get; set; } = null!;
        public Type TargetPropertyType { get; set; } = null!;
        public Type FilterType { get; set; } = null!;

        public Func<object, object> Converter { get; set; } = null!;
        public Expression? SourcePropertyConverter { get; set; }
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
            try
            {
                var dbProperty = dbType.GetProperty(dtoProperty.Name);

                if (dbProperty == null) continue;

                if (dbProperty.PropertyType != dtoProperty.PropertyType) continue;

                var comparisonTypes = TypeNameForComparisonTypes(dbProperty.PropertyType);


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
            catch (Exception e)
            {
                throw new Exception($"Error while adding filter {dtoProperty.Name} as {dtoType.Name}");
            }
        }
    }

    private List<ComparisonType> TypeNameForComparisonTypes(Type type)
    {
        List<ComparisonType> comparisonTypes;
        if (type.IsEnum)
        {
            comparisonTypes = EnumerableExtenders.ComparisonTypes["Enum"];
        }
        else if (type.IsClass && type != typeof(string))
        {
            return new List<ComparisonType>();
            //comparisonTypes = EnumerableExtenders.ComparisonTypes["Class"];
        }
        else if (type.Name == "Nullable`1")
        {
            return TypeNameForComparisonTypes(type.GenericTypeArguments[0]);
        }
        /*else if (type.Namespace == "List`1")
        {
            return new List<ComparisonType>();
        }*/
        else if (type.Name == "String")
        {
            comparisonTypes = EnumerableExtenders.ComparisonTypes["String"];
        }
        else
        {
            comparisonTypes = EnumerableExtenders.ComparisonTypes[type.Name];
        }

        return comparisonTypes;
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