using System;
using System.Collections.Generic;

namespace CategoryService.Core.Entities;

public class Category : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ParentCategoryId { get; set; }         
    public virtual Category? ParentCategory { get; set; }  

    public virtual ICollection<Category> Children { get; set; } = new List<Category>();  

    public string Name { get; set; } = null!;          
    public string? Description { get; set; }            
    public string? Slug { get; set; }                   
    public int SortOrder { get; set; }                    
    public string? ImageUrl { get; set; }              

    public bool IsActive { get; set; } = true;           
    public bool IsVisible { get; set; } = true;          

    public int Level { get; set; }                        
}
