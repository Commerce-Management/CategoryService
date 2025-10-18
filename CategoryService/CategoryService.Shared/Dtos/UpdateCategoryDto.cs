using CategoryService.Shared.Validation;
using Microsoft.AspNetCore.Http;

namespace CategoryService.Shared.Dtos;

public record UpdateCategoryDto(
    string? Name = null,
    string? Description = null,
    Guid? ParentCategoryId = null,
    string? Slug = null,
    int? SortOrder = null,
    
    [AllowedExtensions([".jpg", ".png"])]
    [MaxFileSize(10 * 1024 * 1024)]
    IFormFile? ImageUrl = null,
    
    bool? ClearImageUrl = null,
    bool? IsActive = null,
    bool? IsVisible = null
);