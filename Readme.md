# Pandatech.EFCoreQueryMagic

## Overview

`Pandatech.EFCoreQueryMagic` is a powerful .NET library designed to simplify dynamic filtering, ordering, and
aggregation
of data in Entity Framework Core. It provides a RESTful API experience similar to GraphQL, enabling front-end developers
to build complex data queries easily. This package is especially useful for applications with advanced filtering and
data presentation needs, such as those using the `beautiful-react-table` for rich table functionalities.

## Features

- **Dynamic Filtering**: Apply complex filters dynamically based on user input.
- **Ordering**: Easily sort data by multiple columns.
- **Pagination**: Efficiently paginate data sets to improve performance.
- **Distinct Values**: Retrieve distinct values for a column, useful for preparing filter options.
- **Aggregation**: Perform aggregate operations like sum, average, count, etc.

## Getting Started

### Installation

To install the Pandatech.EFCoreQueryMagic package, run the following command:

```bash
dotnet add package Pandatech.EFCoreQueryMagic
```

**Note**: Our NuGet package has 90% + test coverage.

### Configuration

To make an entity filterable, you need to create a separate filter model class and map it to your entity class.
**Entity Example:**

```csharp
[FilterModel(typeof(UserEntityFilterModel))]
public class UserEntity
{
    public long Id { get; set; }
    public long CounterpartyId { get; set; }
    public Guid? ProfilePictureId { get; set; }
    public bool IsHuman { get; set; }
    public UserStatusType Status { get; set; }
    public bool ForceToChangePassword { get; set; }
    public byte[] PasswordHash { get; set; } = null!;
    public DateTime? PasswordExpirationDate { get; set; }
    public byte[]? Phone { get; set; }
    public bool PhoneIsVerified { get; set; }
    public byte[]? Email { get; set; }
    public bool EmailIsVerified { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public byte[]? TotpSecret { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

**Filter Model Example:**

```csharp
public class UserEntityFilterModel
{
    [MappedToProperty(nameof(UserEntity.Id))]
    public long Id { get; set; }

    [MappedToProperty(nameof(UserEntity.CreatedAt))]
    public DateTime CreationDate { get; set; }

    [MappedToProperty(nameof(UserEntity.CounterpartyId), ConverterType = typeof(FilterPandaBaseConverter))]
    public long CounterpartyId { get; set; }

    [MappedToProperty(nameof(UserEntity.PasswordExpirationDate))]
    public DateTime PasswordExpirationDate { get; set; }

    [MappedToProperty(nameof(UserEntity.FirstName))]
    [Order(1))]
    public string FirstName { get; set; } = null!;

    [Order(2)]
    [MappedToProperty(nameof(UserEntity.LastName))]
    public string LastName { get; set; } = null!;

    [MappedToProperty(nameof(UserEntity.MiddleName))]
    public string? MiddleName { get; set; }

    [MappedToProperty(nameof(UserEntity.Phone), Encrypted = true)]
    public string? PhoneNumber { get; set; }

    [MappedToProperty(nameof(UserEntity.Email), Encrypted = true)]
    public string? EmailAddress { get; set; }
}
```

## Usage

### 1. Retrieve Available Filters

```csharp
public async Task<List<FilterInfo>> GetFiltersAsync()
{
    var tableName = $"{typeof(UserEntityFilterModel).Name}";
    return await FilterExtenders.GetFiltersAsync(typeof(UserEntity).Assembly, tableName);
}
```

### 2. Apply Filters and Ordering

```csharp
public Task<List<UserEntity>> GetUsersAsync(string filterQuery)
{
    return _dbContext
                    .Users
                    .FilterAndOrder(filterQuery)
                    .ToListAsync();
}
```

### 3. Apply Filters, Ordering, and Pagination

```csharp

public Task<PagedResponse<UserEntity>> GetUsersAsync(this IQueryable<UserEntity> query, PageQueryRequest pageQueryRequest, CancellationToken cancellationToken = default)
{
    return _dbContext
                .Users
                .FilterOrderPaginateAsync(pageQueryRequest, cancellationToken);
}
```

### 4. Get Distinct Column Values

```csharp
public Task<List<ColumnDistinctValues>> GetDistinctColumnValuesAsync(ColumnDistinctValueQueryRequest query, CancellationToken cancellationToken = default)
{
    return _dbContext
                .Users
                .GetDistinctColumnValuesAsync(query, cancellationToken);
}
```

### 5. Perform Aggregations

```csharp
public Task<Object?> AggregateAsync(AggregateQueryRequest query, CancellationToken cancellationToken = default)
{
    return _dbContext
                .Users
                .AggregateAsync(query, cancellationToken);
}
```

## Request Models

### FilterQueryRequest

```csharp
public class FilterQueryRequest
{
    public string FilterQuery { get; set; } = "{}";
}
```

### PageQueryRequest

```csharp
public class PageQueryRequest : FilterQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

### ColumnDistinctValueQueryRequest

```csharp
public class ColumnDistinctValueQueryRequest : PageQueryRequest
{
    public string ColumnName { get; init; }
}
```

### AggregateQueryRequest

```csharp
public class AggregateQueryRequest : FilterQueryRequest
{
    public AggregateType AggregateType { get; init; }
    public string ColumnName { get; init; }
}
```

## Response Models

### PagedResponse

```csharp
public record PagedResponse<T>(List<T> Data, int Page, int PageSize, long TotalCount);
```

### ColumnDistinctValues

```csharp
public class ColumnDistinctValues
{
    public List<object> Values { get; set; } = new();
    public long TotalCount { get; set; }
}
```

## Enum Definitions

### ComparisonType

```csharp
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ComparisonType
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith,
    In,
    NotIn,
    IsNotEmpty,
    IsEmpty,
    Between,
    NotContains,
    HasCountEqualTo,
    HasCountBetween,
    IsTrue,
    IsFalse
}
```

### AggregateType

```csharp
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AggregateType
{
    Sum,
    Average,
    Count,
    Min,
    Max
}
```

### MagicQuery Class (Internal) - Client-Side String Representation

```csharp
using System.Text.Json;

internal class MagicQuery
{
    public List<FilterQuery> Filters { get; set; } = new();
    public Ordering Order { get; set; } = new();
}
```

## Exception Handling

Pandatech.EFCoreQueryMagic provides several exceptions to handle various error scenarios during filtering, ordering, and
pagination operations. These exceptions allow you to catch specific errors and handle them accordingly. Additionally, we
recommend using the `Pandatech.ResponseCrafter` NuGet package for a comprehensive response crafting solution.

```csharp
namespace EFCoreQueryMagic.Exceptions
{
    public abstract class FilterException : Exception
    {
        protected FilterException(string message) : base(message) {}
        protected FilterException() {}
    }

    public class ComparisonNotSupportedException : FilterException
    {
        public ComparisonNotSupportedException(string message) : base(message) {}
        public ComparisonNotSupportedException() {}
    }

    public class DefaultOrderingViolation : FilterException {}

    public class MappingException : FilterException
    {
        public MappingException(string message) : base(message) {}
    }

    public class NoOrderingFoundException : FilterException {}

    public class PaginationException : FilterException
    {
        public PaginationException(string message) : base(message) {}
    }

    public class PropertyNotFoundException : FilterException
    {
        public PropertyNotFoundException(string message) : base(message) {}
    }

    public class UnsupportedFilterException : FilterException
    {
        public UnsupportedFilterException(string message) : base(message) {}
    }

    public class UnsupportedValueException : FilterException
    {
        public UnsupportedValueException(string message) : base(message) {}
    }
}
```

## Contributing

We welcome contributions from the community.