namespace PandaTech.Mapper;

public interface IMapping<From, To>
{
    public To Map(From from);

    public List<To> Map(List<From> from);
}