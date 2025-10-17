using CategoryService.Core.Entities;
using CategoryService.Infrastructure.Context;
using CategoryService.Infrastructure.Interfaces.Entities;
using CategoryService.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Infrastructure.Repositories.Entities;

public class CategoryRepository(CategoryDbContext context) : Repository<Category>(context), ICategoryRepository
{
    private IQueryable<Category> GetCategoryQuery() =>
        Entities
            .Include(c => c.ParentCategory)
            .Include(c => c.InverseParentCategory)
            .AsNoTracking();

    public async Task<Category?> GetCategoryByIdAsync(Guid id) =>
        await GetCategoryQuery()
            .SingleOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync() =>
        await GetCategoryQuery()
            .OrderByDescending(category => category.Name)
            .ToListAsync();

    public async Task<Category?> GetCategoryByNameAsync(string name) =>
        await GetCategoryQuery()
            .SingleOrDefaultAsync(c => c.Name == name);
}