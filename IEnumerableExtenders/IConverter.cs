namespace PandaTech.IEnumerableFilters;

public interface IConverter<TModel, TStore>
{
    public TModel Convert(TStore storeValue);
    public TStore Convert(TModel modelValue);
}