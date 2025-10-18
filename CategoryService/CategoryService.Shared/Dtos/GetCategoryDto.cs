namespace CategoryService.Shared.Dtos;

public class GetCategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public Guid? ParentCategoryId { get; init; }
    public string? Slug { get; init; }
    public int Level { get; init; }
    public int SortOrder { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsActive { get; init; }
    public bool IsVisible { get; init; }
    public List<GetCategoryDto> Children { get; init; } = new();
}