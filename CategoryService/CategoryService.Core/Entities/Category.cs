using System;
using System.Collections.Generic;

namespace CategoryService.Core.Entities;

public class Category : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ParentCategoryId { get; set; }          // Для связи с родителем (null, если это корень)
    public virtual Category? ParentCategory { get; set; } // Навигация к родителю

    public virtual ICollection<Category> Children { get; set; } = new List<Category>(); // Список подкатегорий

    public string Name { get; set; } = null!;            // Название категории
    public string? Description { get; set; }             // Описание (опционально)
    public string? Slug { get; set; }                    // Для явного URL (можно генерировать из Name)
    public int SortOrder { get; set; }                   // Для ручного порядка отображения категорий
    public string? ImageUrl { get; set; }                // Картинка/иконка категории

    public bool IsActive { get; set; } = true;           // Категория активна (можно добавить товары)
    public bool IsVisible { get; set; } = true;          // Категория видна на сайте

    public int Level { get; set; }                       // Уровень вложенности (корень = 0)
}
