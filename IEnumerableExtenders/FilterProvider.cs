using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters;

public class FilterProvider
{
    private readonly List<Filter> _filters = new();
    private ILogger<FilterProvider> Logger { get; }

    private readonly Dictionary<FilterKey, string> _expressions = new();

    public record FilterKey
    {
        public Type SourceType { get; set; } = null!;
        public Type TargetType { get; set; } = null!;
        public string SourcePropertyName { get; set; } = null!;
        public string TargetPropertyName { get; set; } = null!;
        public Type SourcePropertyType { get; set; } = null!;
        public Type TargetPropertyType { get; set; } = null!;
        public ComparisonType ComparisonType { get; set; }
    }

    public class Filter
    {
        public Type SourceType { get; set; } = null!;
        public Type TargetType { get; set; } = null!;
        public string SourcePropertyName { get; set; } = null!;
        public Type SourcePropertyType { get; set; } = null!;
        public string TargetPropertyName { get; set; } = null!;
        public Type TargetPropertyType { get; set; } = null!;
        public List<ComparisonType> ComparisonTypes { get; set; } = null!;
        public Func<object, object> Converter { get; set; } = null!;
    }

    public void Add<TSource, TTarget>()
    {
        var sourceType = typeof(TSource);
        var targetType = typeof(TTarget);

        var sourceProperties = sourceType.GetProperties();
        var targetProperties = targetType.GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
            if (targetProperty == null)
            {
                Logger.LogDebug("No matching property found for {SourceProperty}", sourceProperty.Name);
                continue;
            }

            var comparisonTypes = Enum.GetValues<ComparisonType>().ToList();
            if (comparisonTypes.Count == 0)
            {
                Logger.LogDebug("No comparison types found");
                continue;
            }

            var converter = new Func<object, object>(x => x);

            var filter = new Filter
            {
                SourcePropertyName = sourceProperty.Name,
                SourcePropertyType = sourceProperty.PropertyType,
                TargetPropertyName = targetProperty.Name,
                TargetPropertyType = targetProperty.PropertyType,
                ComparisonTypes = comparisonTypes,
                Converter = converter,
                SourceType = typeof(TSource),
                TargetType = typeof(TTarget)
            };

            _filters.Add(filter);

            foreach (var comparisonType in Enum.GetValues<ComparisonType>())
            {
                var key = new FilterKey
                {
                    SourceType = sourceType,
                    TargetType = targetType,
                    SourcePropertyName = sourceProperty.Name,
                    TargetPropertyName = targetProperty.Name,
                    ComparisonType = comparisonType,
                    SourcePropertyType = sourceProperty.PropertyType,
                    TargetPropertyType = targetProperty.PropertyType
                };

                try
                {
                    AddLambdaString(key);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }

    public void Add(Filter filter)
    {
        _filters.RemoveAll(x =>
            x.SourcePropertyName == filter.SourcePropertyName
            && x.TargetType == filter.TargetType);

        _filters.Add(filter);

        foreach (var filterComparisonType in filter.ComparisonTypes)
        {
            AddLambdaString(new FilterKey
            {
                SourceType = filter.SourceType,
                TargetType = filter.TargetType,
                SourcePropertyName = filter.SourcePropertyName,
                TargetPropertyName = filter.TargetPropertyName,
                ComparisonType = filterComparisonType,
                SourcePropertyType = filter.SourcePropertyType,
                TargetPropertyType = filter.TargetPropertyType
            });
        }
    }

    void AddLambdaString(FilterKey key)
    {
        _expressions[key] = BuildLambdaString(key);
    }

    private string BuildLambdaString(FilterKey key)
    {
        return key.TargetPropertyType switch
        {
            { } when key.TargetPropertyType == typeof(string) => BuildStringLambdaString(key),
            { } when _numericTypes.Contains(key.TargetPropertyType) => BuildNumericLambdaString(key),
            { } when key.TargetPropertyType == typeof(bool) => BuildBoolLambdaString(key),
            { } when key.TargetPropertyType == typeof(DateTime) => BuildDateTimeLambdaString(key),
            { } when key.TargetPropertyType.IsEnum => BuildEnumLambdaString(key),
            { } when key.TargetPropertyType == typeof(Guid) => BuildGuidLambdaString(key),
            { } when key.TargetPropertyType is { IsClass: true, IsGenericType: false } => BuildClassLambdaString(key),
            { } when key.TargetPropertyType == typeof(DateOnly) => BuildDateTimeLambdaString(key),
            // and nullables 
            { } when key.TargetPropertyType == typeof(int?) => BuildNumericLambdaString(key),
            { } when key.TargetPropertyType == typeof(double?) => BuildNumericLambdaString(key),
            { } when key.TargetPropertyType == typeof(decimal?) => BuildNumericLambdaString(key),
            { } when key.TargetPropertyType == typeof(bool?) => BuildBoolLambdaString(key),
            { } when key.TargetPropertyType == typeof(DateTime?) => BuildDateTimeLambdaString(key),
            { } when key.TargetPropertyType == typeof(Guid?) => BuildGuidLambdaString(key),
            { } when key.TargetPropertyType is { IsClass: true, IsGenericType: false }  => BuildClassLambdaString(key),
            { } when key.TargetPropertyType == typeof(DateOnly?) => BuildDateTimeLambdaString(key),
            // lists 
            { } when key.TargetPropertyType.IsGenericType && key.TargetPropertyType.GetGenericTypeDefinition() == typeof(List<>) => BuildListLambdaString(key),
            _ => throw new Exception($"Unsupported type {key.TargetPropertyType}")
        };
    }

    private string BuildClassLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName}.Id == @{0}",
            ComparisonType.NotEqual => $"{key.TargetPropertyName}.Id != @{0}",
            ComparisonType.In => $"@{0}.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@{0}.Contains({key.TargetPropertyName})",
            _ => throw new Exception(
                $"Unsupported comparison type {key.ComparisonType} for type {key.TargetPropertyType}")
        };
    }

    private string BuildListLambdaString(FilterKey key)
    {
        // Check for value types and enums and strings 
        return key.ComparisonType switch
        {
            ComparisonType.Contains => $"{key.TargetPropertyName}.Any(x => x == @{0})",
            ComparisonType.NotContains => $"!{key.TargetPropertyName}.Any(x => x == @{0})",
            _ => throw new Exception(
                $"Unsupported comparison type {key.ComparisonType} for type {key.TargetPropertyType}")
        };
    }

    private string BuildListLambdaStringForClass(FilterKey key)
    {
        var targetPrimaryKeyAttributeData = key.TargetType.CustomAttributes
            .FirstOrDefault(x => x.AttributeType == typeof(PrimaryKeyAttribute)) ??
                                           throw new Exception($"No primary key found for {key.TargetType.Name}");

        var propertyNames = targetPrimaryKeyAttributeData.ConstructorArguments
            .Select(x => x.Value!.ToString()!)
            .ToList();

        var expression = new StringBuilder();
        expression.Append($"{key.TargetPropertyName}.Any(x => ");

        for (var i = 0; i < propertyNames.Count; i++)
        {
            var propertyName = propertyNames[i];
            expression.Append($"x.{propertyName} == @{i}");
            if (i != propertyNames.Count - 1)
            {
                expression.Append(" && ");
            }
        }

        expression.Append(')');

        return expression.ToString();
    }
    
    
    private string BuildGuidLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            _ => throw new Exception($"Unsupported comparison type {key.ComparisonType}")
        };
    }

    private string BuildEnumLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            _ => throw new Exception($"Unsupported comparison type {key.ComparisonType}")
        };
    }

    readonly List<Type> _numericTypes = new()
    {
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal)
    };

    public FilterProvider(ILogger<FilterProvider> logger)
    {
        Logger = logger;
    }


    private string BuildBoolLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0",
            ComparisonType.IsTrue => $"{key.TargetPropertyName}",
            ComparisonType.IsFalse => $"{key.TargetPropertyName}",
            _ => throw new Exception($"Unsupported comparison type {key.ComparisonType}")
        };
    }

    private string BuildDateTimeLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0",
            ComparisonType.GreaterThan => $"{key.TargetPropertyName} > @0",
            ComparisonType.GreaterThanOrEqual => $"{key.TargetPropertyName} >= @0",
            ComparisonType.LessThan => $"{key.TargetPropertyName} < @0",
            ComparisonType.LessThanOrEqual => $"{key.TargetPropertyName} <= @0",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            ComparisonType.Between => $"{key.TargetPropertyName} >= @0 && {key.TargetPropertyName} <= @1",
            _ => throw new ComparisonNotSupportedException(key)
        };
    }

    private string BuildNumericLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0",
            ComparisonType.GreaterThan => $"{key.TargetPropertyName} > @0",
            ComparisonType.GreaterThanOrEqual => $"{key.TargetPropertyName} >= @0",
            ComparisonType.LessThan => $"{key.TargetPropertyName} < @0",
            ComparisonType.LessThanOrEqual => $"{key.TargetPropertyName} <= @0",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            ComparisonType.Between => $"{key.TargetPropertyName} >= @0 && {key.TargetPropertyName} <= @1",
            _ => throw new ComparisonNotSupportedException(key)
        };
    }

    private string BuildStringLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0",
            ComparisonType.Contains => $"{key.TargetPropertyName}.Contains(@0)",
            ComparisonType.StartsWith => $"{key.TargetPropertyName}.StartsWith(@0)",
            ComparisonType.EndsWith => $"{key.TargetPropertyName}.EndsWith(@0)",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            ComparisonType.IsNotEmpty => $"{key.TargetPropertyName}.Length > 0",
            ComparisonType.IsEmpty => $"{key.TargetPropertyName}.Length == 0",
            ComparisonType.NotContains => $"!{key.TargetPropertyName}.Contains(@0)",
            ComparisonType.HasCountEqualTo => $"{key.TargetPropertyName}.Count() == @0",
            ComparisonType.HasCountBetween =>
                $"{key.TargetPropertyName}.Count() >= @0 && {key.TargetPropertyName}.Count() <= @0",
            _ => throw new ArgumentOutOfRangeException()
        };
    }


    public Filter GetFilter(string sourcePropertyName, ComparisonType comparisonType, Type targetType)
    {
        var filter = _filters.FirstOrDefault(x =>
            x.SourcePropertyName == sourcePropertyName && x.ComparisonTypes.Contains(comparisonType) &&
            x.TargetType == targetType);

        if (filter == null)
        {
            throw new PropertyNotFoundException(sourcePropertyName);
        }

        return filter;
    }

    public string GetFilterLambda(string filterDtoPropertyName, ComparisonType filterDtoComparisonType,
        Type targetTable)
    {
        var key =
            _expressions.Keys.FirstOrDefault(x =>
                x.SourcePropertyName == filterDtoPropertyName && x.ComparisonType == filterDtoComparisonType &&
                x.TargetType == targetTable) ?? throw new PropertyNotFoundException(filterDtoPropertyName);
        return _expressions.TryGetValue(key, out var expression)
            ? expression
            : throw new PropertyNotFoundException(filterDtoPropertyName);
    }
}