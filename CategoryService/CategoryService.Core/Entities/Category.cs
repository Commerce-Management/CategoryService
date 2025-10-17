namespace CategoryService.Core.Entities;

public class Category : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? ParentCategoryId { get; set; } 

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();
    
    public virtual Category? ParentCategory { get; set; }
}