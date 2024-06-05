namespace EFCoreQueryMagic.Test.Entities;

public class ItemTypeMapping
{
    public long Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid ItemTypeId { get; set; }

    public Item Item { get; set; }
    public ItemType ItemType { get; set; }
}