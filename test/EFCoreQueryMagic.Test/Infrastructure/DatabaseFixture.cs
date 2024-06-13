using System.Text;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;
using Pandatech.Crypto;
using Random = System.Random;

namespace EFCoreQueryMagic.Test.Infrastructure;

public class DatabaseFixture : IDisposable
{
    public TestDbContext Context { get; private set; }
    public Aes256 Aes256 { get; private set; }

    public DatabaseFixture()
    {
        Aes256 = new Aes256(new Aes256Options
        {
            Key = "M5pfvJCKBwpJdA7YfeX3AkAKJmfBf4piybEPDtWKWw4="
        });

        Context = TestDbContext.CreateNewInMemoryContext();

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        SeedData(Context);
    }

    private void SeedData(TestDbContext context)
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Categories = [CategoryType.Average, CategoryType.Good] },
            new() { Id = 2, Categories = [CategoryType.Poor, CategoryType.Average] }
        };

        context.Categories.AddRange(categories);

        var customers = new List<Customer>
        {
            new()
            {
                FirstName = ConvertToByteArray("FirstName1"),
                LastName = GenerateBytesFromString("LastName1").Select(x => (int)x).ToArray(),
                SomeByteArray = GenerateBytesFromString("MiddleName1"),
                Email = "customer1@example.com",
                Types = [CustomerType.Seller, CustomerType.Owner],
                Statuses = [CustomerStatus.Active],
                Age = 18,
                PhoneNumber = "+37411223344",
                Average = 12M,
                Maximum = 150M,
                TotalOrders = 1,
                BirthDay = new DateTime(2024, 03, 10).ToUniversalTime(),
                SocialId = ConvertToByteArray("1234567890"),
                CreatedAt = new DateTime(2024, 03, 10).ToUniversalTime(),
                Category = categories[0]
            },
            new()
            {
                FirstName = ConvertToByteArray("FirstName2"),
                LastName = GenerateBytesFromString("LastName2").Select(x => (int)x).ToArray(),
                SomeByteArray = null,
                Email = "customer2@example.com",
                Types = [CustomerType.Seller, CustomerType.Other],
                Statuses = null,
                Age = null,
                PhoneNumber = null,
                Average = 10M,
                Maximum = null,
                TotalOrders = 10,
                BirthDay = null,
                SocialId = null,
                CreatedAt = new DateTime(2024, 03, 11).ToUniversalTime(),
                Category = categories[1]
            }
        };

        context.AddRange(customers);

        var orders = new List<Order>
        {
            new()
            {
                Id = 1, TotalAmount = 100.50m, MinSize = 1L, Discount = 2L,
                PaymentStatus = PaymentStatus.Completed, CancellationStatus = CancellationStatus.No,
                Paid = true, Returned = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1), CustomerId = 1
            },
            new()
            {
                Id = 2, TotalAmount = 200.00m, MinSize = 1.5M, Discount = null,
                PaymentStatus = PaymentStatus.Pending, CancellationStatus = CancellationStatus.Yes,
                Paid = false, Returned = null,
                CreatedAt = DateTime.UtcNow, CustomerId = 2,
            }
        };

        context.Orders.AddRange(orders);

        var item1 = new Item
        {
            Id = Guid.NewGuid(), IdNullable = Guid.NewGuid(),
            Price = 1500L, DiscountedPrice = null,
            MinPrice = 1000L, MaxPrice = 5000L, MinQuantity = 1,
            MaxQuantity = 100, UShort = 1, UShortNullable = 2,
            UInt = 3, UIntNullable = 4, ULong = 5, UlongNullable = 6,
            AvailablePeriod = new TimeSpan(10, 0, 0),
            UnavailablePeriod = new TimeSpan(1, 0, 0),
            CreatedAt = DateTime.UtcNow,
            Char = 'A', CharNullable = 'B',
            Byte = new byte(), ByteNullable = new byte(),
            SByte = new sbyte(), SByteNullable = new sbyte(),
            DateTimeOffset = DateTimeOffset.UtcNow,
            DateTimeOffsetNullable = DateTimeOffset.UtcNow,
            DateOnly = new DateOnly(2024, 03, 10),
            DateOnlyNullable = new DateOnly(2024, 03, 11),
            TimeOnly = new TimeOnly(12, 25, 00),
            TimeOnlyNullable = new TimeOnly(12, 30, 00),
            OrderId = 1, ListString = ["1", "2", "3"], ListStringNullable = ["4", "5", "6"]
        };
        var item2 = new Item
        {
            Id = Guid.NewGuid(), IdNullable = null, Price = 3500L,
            DiscountedPrice = 2500L, MinPrice = 2000L,
            MaxPrice = null, MinQuantity = 1, MaxQuantity = null,
            UShort = 1, UShortNullable = null, UInt = 3,
            UIntNullable = null, ULong = 5, UlongNullable = null,
            AvailablePeriod = new TimeSpan(10, 0, 0),
            UnavailablePeriod = null, CreatedAt = DateTime.UtcNow,
            Char = 'C', CharNullable = null, Byte = new byte(),
            ByteNullable = null,
            SByte = new sbyte(), SByteNullable = null, DateTimeOffset = DateTimeOffset.UtcNow,
            DateTimeOffsetNullable = null, DateOnly = new DateOnly(2024, 03, 20),
            DateOnlyNullable = null, TimeOnly = new TimeOnly(12, 35, 00),
            TimeOnlyNullable = null,
            OrderId = 2, ListString = ["1", "2", "3"], ListStringNullable = null
        };
        var item3 = new Item
        {
            Id = Guid.NewGuid(), IdNullable = null, Price = 3500L,
            DiscountedPrice = 2500L, MinPrice = 2000L,
            MaxPrice = null, MinQuantity = 1, MaxQuantity = null,
            UShort = 1, UShortNullable = null, UInt = 3,
            UIntNullable = null, ULong = 5, UlongNullable = null,
            AvailablePeriod = new TimeSpan(10, 0, 0),
            UnavailablePeriod = null, CreatedAt = DateTime.UtcNow,
            Char = 'C', CharNullable = null, Byte = new byte(),
            ByteNullable = null,
            SByte = new sbyte(), SByteNullable = null, DateTimeOffset = DateTimeOffset.UtcNow,
            DateTimeOffsetNullable = null, DateOnly = new DateOnly(2024, 03, 20),
            DateOnlyNullable = null, TimeOnly = new TimeOnly(12, 35, 00),
            TimeOnlyNullable = null,
            OrderId = null, ListString = ["1", "2", "3"], ListStringNullable = null
        };
        var items = new List<Item>
        {
            item1,
            item2,
            item3
        };

        context.Items.AddRange(items);

        var categoryNames = new List<CategoryName>
        {
            new()
            {
                Id = 1, NameAm = "Թեստ", NameRu = "Тест", NameEn = "Test"
            },
            new()
            {
                Id = 2, NameAm = "Թեստ 2", NameRu = "Тест 2", NameEn = "Test 2"
            }
        };

        context.CategoryNames.AddRange(categoryNames);

        var itemType1 = new ItemType
        {
            Id = Guid.NewGuid(),
            NameAm = "Բարև 1",
            NameRu = "Привет 1",
            NameEn = "Hello 1"
        };
        var itemType2 = new ItemType
        {
            Id = Guid.NewGuid(),
            NameAm = "Բարև 2",
            NameRu = "Привет 2",
            NameEn = "Hello 2"
        };
        var itemType3 = new ItemType
        {
            Id = Guid.NewGuid(),
            NameAm = "Բարև 3",
            NameRu = "Привет 3",
            NameEn = "Hello 3"
        };
        var itemTypes = new List<ItemType>
        {
            itemType1,
            itemType2,
            itemType3
        };

        context.ItemTypes.AddRange(itemTypes);

        var itemTypeMappings = new List<ItemTypeMapping>
        {
            new()
            {
                Id = 1,
                Item = item1,
                ItemType = itemType1
            },
            new()
            {
                Id = 2,
                Item = item1,
                ItemType = itemType2
            },
            new()
            {
                Id = 3,
                Item = item1,
                ItemType = itemType3
            },
            new()
            {
                Id = 4,
                Item = item2,
                ItemType = itemType2
            },
            new()
            {
                Id = 5,
                Item = item3,
                ItemType = itemType3
            }
        };

        context.ItemTypeMappings.AddRange(itemTypeMappings);

        context.SaveChanges();
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        var randomBytes = new byte[length];
        new Random().NextBytes(randomBytes);
        return randomBytes;
    }

    private static byte[] GenerateBytesFromString(string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }

    private byte[] ConvertToByteArray(string value)
    {
        return Aes256.EncryptWithoutHash(value);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}