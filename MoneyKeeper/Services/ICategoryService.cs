using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface ICategoryService
{
    Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request, int userId);
    Task<List<CategoryResponse>> GetCategoriesAsync(int userId);
}
