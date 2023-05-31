namespace PandaTech.IEnumerableFilters;

[AttributeUsage(AttributeTargets.Property)]
public class FilterValueConverter<TModel, TStore> : Attribute
{
    public FilterValueConverter(Type converterType)
    {
        Converter = Activator.CreateInstance(converterType) as IConverter<TModel, TStore> ??
                    throw new Exception("Could not create converter");
    }

    private IConverter<TModel, TStore> Converter { get; }
}