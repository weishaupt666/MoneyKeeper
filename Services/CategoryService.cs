using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category> Create(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<List<Category>> GetAll()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();
        return categories;
    }
}
