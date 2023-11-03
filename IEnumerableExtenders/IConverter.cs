namespace PandaTech.IEnumerableFilters;

public interface IConverter<TFrom, TTo>
{
    public TTo ConvertTo(TFrom from);

    public TFrom ConvertFrom(TTo to);
}