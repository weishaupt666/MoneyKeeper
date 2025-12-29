using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface ICategoryService
{
    Task<Category> Create(CreateCategoryRequest request);
    Task<List<Category>> GetAll();
}
