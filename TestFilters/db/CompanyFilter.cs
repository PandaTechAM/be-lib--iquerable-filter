using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Enums;

namespace TestFilters.db;

public class CompanyFilter
{
    [MappedToProperty(nameof(Company.Id))]
    [Order(2)]
    public long Id { get; set; }

    [MappedToProperty(nameof(Company.Age))]
    [Order(direction: OrderDirection.Descending)]
    public long Age { get; set; }
    
    [MappedToProperty(nameof(Company.NullableString))]
    public string? NullableString { get; set; }

    [MappedToProperty(nameof(Company.Name))]
    public string Name { get; set; } = null!;

    [MappedToProperty(nameof(Company.Type))]
    public string Type { get; set; } = null!;

    [MappedToProperty(nameof(Company.Types))]
    public string Types { get; set; } = null!;

    [MappedToProperty(nameof(Company.IsEnabled))]
    public bool IsEnabled { get; set; }

    [MappedToProperty(nameof(Company.NameEncrypted), Encrypted = true, Sortable = false)]
    public string NameEncrypted { get; set; } = null!;

    [MappedToProperty(nameof(Company.Info), nameof(Company.Info.Name))]
    public string InfoName { get; set; } = null!;
    
    [MappedToProperty(nameof(Company.SomeClass), nameof(Company.SomeClass.NullableString))]
    public string? SomeClassN { get; set; } = null!;
    
    [MappedToProperty(nameof(Company.SomeClass), nameof(Company.SomeClass.NameEncrypted), Encrypted = true)]
    public string? SomeClassNn { get; set; } = null!;
    
    [MappedToProperty(nameof(Company.NullableAge))]
    public long? NullableAge { get; set; }
}

public class SomeClassConverter : IConverter<string, SomeClass>
{
    public DbContext Context { get; set; }
    public string ConvertFrom(SomeClass from) => from.Name;
    
    public SomeClass ConvertTo(string from)
    {
        var result = Context.Set<SomeClass>().FirstOrDefault(x => x.Name.ToLower() == from.ToLower());
        var a = 1;
        return result;
    }
}


public abstract class FilterRules<TModel>
{

    private List<IRule> _rules = new();

    private interface IRule;
    
    
    
    protected class Rule<TAccept, TTarget> : IRule
    {   
        public Rule<TAccept, TTarget> ConvertTo(Expression<Func<TAccept, TTarget>> converter)
        {
            return this;
        }
        
        public Rule<TAccept, TTarget> ConvertFrom(Expression<Func<TTarget, TAccept>> converter)
        {
            return this;
        }
        
        public Rule<TAccept, TTarget> AddDefaultComparisons<T>()
        {
            return this;
        }

        public Rule<TAccept, TTarget> AddComparison(ComparisonType comparisonType, Func<TTarget, TTarget[], bool> func)
        {
            return this;
        }
    };

    protected Rule<TAccept, TTarget> RuleFor<TAccept, TTarget>(string name,  Expression<Func<TModel, TTarget>> expression)
    {
        var rule = new Rule<TAccept, TTarget>();
        _rules.Add(rule);
        
        return rule;
    }
    
    protected Rule<TTarget, TTarget> RuleFor<TTarget>(string name,  Expression<Func<TModel, TTarget>> expression)
    {
        var rule = new Rule<TTarget, TTarget>();
        _rules.Add(rule);
        
        return rule;
    }
}

public class CompanyFilterRules : FilterRules<Company>
{
    public CompanyFilterRules()
    {
        RuleFor<string, long>("Id", x => x.Id)
            .ConvertTo(x => long.Parse(x))
            .ConvertFrom(x => x.ToString())
            .AddDefaultComparisons<long>();
        
        RuleFor("Age", x => x.Age)
            .AddComparison(ComparisonType.Between, (x, p) => x > p[0] && x < p[1]);
    }
}

