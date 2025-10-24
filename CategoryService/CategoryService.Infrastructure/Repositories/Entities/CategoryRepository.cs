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
            .AsNoTracking();

    public async Task<Category?> GetCategoryByIdAsync(Guid id) =>
        await GetCategoryQuery()
            .SingleOrDefaultAsync(c => c.Id == id);

    public async Task<Category?> GetDetailCategoryById(Guid id)
    {
        var category = await Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return null;

        var current = category;
        while (current.ParentCategoryId.HasValue)
        {
            var parent = await Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == current.ParentCategoryId.Value);

            if (parent == null)
                break; 

            current.ParentCategory = parent;

            current = parent;
        }

        category.Children = await Entities.Where(c => c.ParentCategoryId == category.Id).AsNoTracking().ToListAsync();

        return category;
    }

    

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync() =>
        await GetCategoryQuery()
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .ToListAsync();

    public async Task<Category?> GetCategoryByNameAsync(string name) =>
        await GetCategoryQuery()
            .SingleOrDefaultAsync(c => c.Name == name);
}