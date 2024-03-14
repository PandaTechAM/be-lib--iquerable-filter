using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Dtos;

public class CustomerDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    public List<CategoryType> Categories { get; set; } = null!;
}