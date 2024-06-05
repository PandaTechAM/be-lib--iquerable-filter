namespace EFCoreQueryMagic.Test.Entities;

public class ItemType
{
    public Guid Id { get; set; }
    public string NameAm { get; set; }
    public string NameRu { get; set; }
    public string NameEn { get; set; }

    public ICollection<ItemTypeMapping> ItemTypeMappings { get; set; }
}