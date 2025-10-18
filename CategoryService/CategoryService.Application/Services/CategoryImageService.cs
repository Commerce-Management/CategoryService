using CategoryService.Core.Interfaces;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CategoryService.Application.Services;

public class CategoryImageService(IConfiguration configuration) : ICategoryImageService
{
    private readonly ImagekitClient _imagekitClient = new (configuration["ImageKit:PublicKey"], configuration["ImageKit:PrivateKey"], configuration["ImageKit:UrlEndPoint"]);
    
    public async  Task<string> UploadCategoryImageAsync(IFormFile imageData)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("Image data is null or empty.", nameof(imageData));

        using var memoryStream = new MemoryStream();
        await imageData.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var ob = new FileCreateRequest
        {
            file = fileBytes,
            fileName = Guid.NewGuid().ToString()
        };
        
        var result = await _imagekitClient.UploadAsync(ob);
        return result.url;
    }

    public Task<bool> DeleteCategoryImageAsync(string imageUrl)
    {
        throw new NotImplementedException();
    }
}