using CategoryService.Core.Entities;

namespace CategoryService.Infrastructure.Context;

public class CategoryDbContext : DbContext
{
    public CategoryDbContext()
    {
    }

    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<Category> Categories { get; set; }
    
}