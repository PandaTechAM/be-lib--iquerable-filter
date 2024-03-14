using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

public class Category
{
    public int Id { get; set; }
    public List<CategoryType> Categories { get; set; } = null!;

    public List<Customer> Customers { get; set; } = null!;
}