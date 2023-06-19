using System.ComponentModel.Design;
using System.Linq.Expressions;

namespace PandaTech.Mapper;

public class Mapper
{
    private readonly IServiceProvider _serviceProvider;

    public Mapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TTo Map<TFrom, TTo>(TFrom from)
    {
        if (_serviceProvider.GetService(typeof(IMapping<TFrom, TTo>)) is IMapping<TFrom, TTo> mapping) return mapping.Map(from);
        throw new Exception("Mapping not found!");
    }
}