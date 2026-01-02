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

    public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request, int userId)
    {
        bool exists = await _context.Categories
            .AnyAsync(c => c.Name == request.Name &&
            (c.UserId == userId || c.UserId == null));

        if (exists)
        {
            throw new ArgumentException($"Category '{request.Name}' already exists.");
        }

        var category = new Category
        {
            Name = request.Name,
            UserId = userId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            IsSystem = false
        };
    }

    public async Task<List<CategoryResponse>> GetCategoriesAsync(int userId)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId || c.UserId == null)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                IsSystem = c.UserId == null
            })
            .ToListAsync();

        return categories;
    }
}
