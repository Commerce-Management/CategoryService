using CategoryService.Core.Interfaces;
using CategoryService.Infrastructure.Interfaces.Entities;
using CategoryService.Shared.Protos.GrpcCategoryService;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CategoryService.Infrastructure.gRPC;

public class GrpcCategoryService : CategoryService.Shared.Protos.GrpcCategoryService.CategoryService.CategoryServiceBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GrpcCategoryService> _logger;

    public GrpcCategoryService(ICategoryRepository categoryRepository, ILogger<GrpcCategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public override async Task<GetCategoryByIdResponse> GetCategoryById(GetCategoryByIdRequest request,
        ServerCallContext context)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.CategoryId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "CategoryId is required"));

        if (!Guid.TryParse(request.CategoryId, out var categoryId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "CategoryId invalid"));

        try
        {
           
            var category = await _categoryRepository.GetDetailCategoryById(categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category {CategoryId} not found", categoryId);
                throw new RpcException(new Status(StatusCode.NotFound, "Category not found"));
            }

            if (!category.IsActive)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Category is not active"));

            if (!category.IsVisible)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Category is not visible"));

        
            var dto = new CategoryDto
            {
                Id = category.Id.ToString(),
                Name = category.Name ?? string.Empty,
                Description = category.Description ?? string.Empty,
                Slug = category.Slug ?? string.Empty,
                Level = category.Level,
                SortOrder = category.SortOrder,
                ImageUrl = category.ImageUrl ?? string.Empty,
                IsActive = category.IsActive,
                IsVisible = category.IsVisible
            };

       
            if (category.ParentCategoryId.HasValue)
            {
         
                var parent = category.ParentCategory ??
                             await _categoryRepository.GetByIdAsync(category.ParentCategoryId.Value);
                if (parent != null)
                {
                    dto.ParentCategory = new CategoryShortDto
                    {
                        Id = parent.Id.ToString(),
                        Name = parent.Name ?? string.Empty,
                        Level = parent.Level
                    };
                }
            }

      
            if (category.Children != null && category.Children.Any())
            {
                foreach (var child in category.Children)
                {
                    dto.Children.Add(new CategoryShortDto
                    {
                        Id = child.Id.ToString(),
                        Name = child.Name ?? string.Empty,
                        Level = child.Level
                    });
                }
            }

    
            var parentsStack = new List<CategoryShortDto>();
            var curParentId = category.ParentCategoryId;

            while (curParentId.HasValue)
            {
                var p = await _categoryRepository.GetByIdAsync(curParentId.Value);
                if (p == null) break;

                parentsStack.Add(new CategoryShortDto
                {
                    Id = p.Id.ToString(),
                    Name = p.Name ?? string.Empty,
                    Level = p.Level
                });

              
                curParentId = p.ParentCategoryId;
            }

           
            parentsStack.Reverse();
            foreach (var parentDto in parentsStack)
                dto.Parents.Add(parentDto);

       
            return new GetCategoryByIdResponse { Category = dto };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {CategoryId}", categoryId);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while processing your request"));
        }
    }
    
    public override async Task<GetCategoriesByIdsResponse> GetCategoriesByIds(
        GetCategoriesByIdsRequest request,
        ServerCallContext context)
    {
        var categoryIds = request.CategoryIds
            .Where(id => Guid.TryParse(id, out _))
            .Select(Guid.Parse)
            .ToList();

        var categories = await _categoryRepository.GetCategoriesByIdsAsync(categoryIds);

        var response = new GetCategoriesByIdsResponse();
    
        foreach (var category in categories)
        {
            response.Categories.Add(new CategorySimple
            {
                Id = category.Id.ToString(),
                Name = category.Name,
                Level = category.Level
            });
        }

        return response;
    }
}