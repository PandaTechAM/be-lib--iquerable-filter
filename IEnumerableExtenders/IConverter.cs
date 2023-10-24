namespace PandaTech.IEnumerableFilters;

public interface IConverter<in TFrom, out TTo>
{
    public TTo Convert(TFrom from);
}