namespace EFCoreQueryMagic.Extensions;

internal static class EnumerableExtensions
{
    internal static int IndexOf<T>(this IEnumerable<T> enumerable, T value)
    {
        var index = 0;
        
        foreach (var item in enumerable)
        {
            if (EqualityComparer<T>.Default.Equals(item, value))
            {
                return index;
            }
            
            index++;
        }
        
        return -1;
    }
    
    internal static IEnumerable<T> RemoveAt<T>(this IEnumerable<T> enumerable, int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        var currentIndex = 0;
        foreach (var item in enumerable)
        {
            if (currentIndex != index)
            {
                yield return item;
            }
            currentIndex++;
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, currentIndex);
    }
    
    internal static IEnumerable<T> Insert<T>(this IEnumerable<T> enumerable, int index, T value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        var currentIndex = 0;
        var itemInserted = false;

        foreach (var item in enumerable)
        {
            if (currentIndex == index)
            {
                yield return value;
                itemInserted = true;
            }
            
            yield return item;
            currentIndex++;
        }

        if (!itemInserted && index == currentIndex)
        {
            yield return value;
        }
        else if (!itemInserted)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
    
    internal static IEnumerable<object>? MoveNullToTheBeginning(this IEnumerable<object>? enumerable)
    {
        if (enumerable is null) return enumerable;
        
        var nullIndex = enumerable.IndexOf(null);
        if (nullIndex > -1)
        {
            enumerable = enumerable.RemoveAt(nullIndex);
            enumerable = enumerable.Insert(0, null);
        }
        
        return enumerable;
    }
    
    internal static List<object>? MoveNullToTheBeginning(this List<object>? enumerable)
    {
        if (enumerable is null) return enumerable;
        
        var nullIndex = enumerable.IndexOf(null);
        if (nullIndex > -1)
        {
            enumerable.RemoveAt(nullIndex);
            enumerable.Insert(0, null);
        }
        
        return enumerable;
    }
}