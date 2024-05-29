namespace EFCoreQueryMagic.Dto.Public;

public record PagedResponse<T>(List<T> Data, int Page, int PageSize, long TotalCount);