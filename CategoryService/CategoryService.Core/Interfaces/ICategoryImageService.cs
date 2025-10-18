using Microsoft.AspNetCore.Http;

namespace CategoryService.Core.Interfaces;

public interface ICategoryImageService
{
    public Task<string> UploadCategoryImageAsync(IFormFile imageData);
    public Task<bool> DeleteCategoryImageAsync(string imageUrl);
}