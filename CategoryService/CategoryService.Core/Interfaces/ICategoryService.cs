using CategoryService.Shared.Dtos;

namespace CategoryService.Core.Interfaces;

public interface ICategoryService
{
    public Task<IEnumerable<GetCategoryDto>> GetAllCategories();
    public Task<GetCategoryDto> GetCategoryById(Guid id);
    public Task<GetCategoryDto?> GetCategoryByName(string name);
    public Task<GetCategoryDto> CreateCategory(CreateCategoryDto createCategoryDto);
    public Task<bool> UpdateCategory(Guid id,UpdateCategoryDto updateCategoryDto);
    public Task<bool> DeleteCategory(Guid id);
}