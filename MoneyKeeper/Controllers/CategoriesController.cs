using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Extensions;
using MoneyKeeper.Models;
using MoneyKeeper.Services;

namespace MoneyKeeper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var userId = User.GetUserId();
        var category = await _categoryService.CreateCategoryAsync(request, userId);
        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll()
    {
        var userId = User.GetUserId();
        var categories = await _categoryService.GetCategoriesAsync(userId);
        return Ok(categories);
    }
}
