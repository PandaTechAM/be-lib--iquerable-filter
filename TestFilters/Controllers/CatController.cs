using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Extensions;
using TestFilters.Controllers.Models;

namespace TestFilters.Controllers;

[Route("[controller]")]
public class CatController : Controller
{
    private readonly Context _context;
    private readonly Aes256 _aes256;

    private readonly IServiceProvider _serviceProvider;

    public CatController(Context context, IServiceProvider serviceProvider, Aes256 aes256)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _aes256 = aes256;
    }

    [HttpGet("")]
    public async Task<PagedResult<CatDto>> GetCats(string requestString = "{}", int page = 1, int pageSize = 20)
    {
        var request = GetDataRequest.FromString(requestString);

        var query = _context.Cats
                .Include(x => x.Types)
                .ApplyFilters<Cat>(request.Filters)
            ; //.Where(x => PostgresDbContext.substr(x.SomeBytes, 1, 64) == hash);

        var data = (await query.Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync()).Select(x => new CatDto
        {
            Id = x.Id, Name = x.Name, Age = x.Age, CatType = x.Types.Name,
            EncryptedString = _aes256.Decrypt(x.SomeBytes) ?? ""
        }).ToList();

        return new PagedResult<CatDto>() { Data = data };
    }
}