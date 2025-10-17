using CategoryService.Core.Entities;
using CategoryService.Infrastructure.Interfaces.Base;

namespace CategoryService.Infrastructure.Interfaces.Entities;

public interface ICategoryRepository : IRepository<Category>
{
    public Task<Category?> GetCategoryByIdAsync(Guid id);
    public Task<IEnumerable<Category>> GetAllCategoriesAsync();
    public Task<Category?> GetCategoryByNameAsync(string name);
}