using AutoMapper;
using CategoryService.Core.Entities;
using CategoryService.Core.Interfaces;
using CategoryService.Infrastructure.Interfaces.Base;
using CategoryService.Infrastructure.Interfaces.Entities;
using CategoryService.Shared.Dtos;

namespace CategoryService.Application.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IMapper mapper, 
    ICategoryImageService categoryImageService,
    IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<IEnumerable<GetCategoryDto>> GetAllCategories() =>
        mapper.Map<IEnumerable<GetCategoryDto>>(await categoryRepository.GetAllCategoriesAsync());

    public async Task<GetCategoryDto> GetCategoryById(Guid id) =>
        mapper.Map<GetCategoryDto>(await categoryRepository.GetCategoryByIdAsync(id));

    public async Task<GetCategoryDto?> GetCategoryByName(string name) =>
        mapper.Map<GetCategoryDto>(await categoryRepository.GetCategoryByNameAsync(name));

    public async Task<GetCategoryDto> CreateCategory(CreateCategoryDto categoryDto)
    {
        try
        {
            var categoryEntity = mapper.Map<Category>(categoryDto);

            if (categoryDto.ImageUrl is {Length: > 0})
            {
                var url = await categoryImageService.UploadCategoryImageAsync(categoryDto.ImageUrl);
                categoryEntity.ImageUrl = url;  
            }
            else
            {
                categoryEntity.ImageUrl = null;  
            }

            await unitOfWork.BeginTransactionAsync();

            var category = await categoryRepository.InsertAsync(categoryEntity);

            await unitOfWork.CommitTransactionAsync();
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GetCategoryDto>(category);
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> UpdateCategory(Guid id, UpdateCategoryDto categoryDto)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);
        
        if (category is null)
            return false;

        if (categoryDto.ImageUrl != null && categoryDto.ImageUrl.Length > 0)
        {
            var url = await categoryImageService.UploadCategoryImageAsync(categoryDto.ImageUrl);
            category.ImageUrl = url;
        }
        else if (categoryDto.ClearImageUrl == true)
        {
            category.ImageUrl = null;
        }
        
        mapper.Map(categoryDto, category);
        categoryRepository.Update(category);
        var result = await categoryRepository.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteCategory(Guid id)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);
        
        if (category is null)
            return false;
        
        categoryRepository.Delete(category);
        
        var result = await categoryRepository.SaveChangesAsync();
        return result > 0;
    }
}