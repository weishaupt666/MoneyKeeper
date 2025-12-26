using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(category);
    }
}
