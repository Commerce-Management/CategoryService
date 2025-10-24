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
            // Получаем детальную категорию (саму сущность)
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

            // map main dto
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

            // parent (immediate)
            if (category.ParentCategoryId.HasValue)
            {
                // если GetDetailCategoryById уже вернул ParentCategory populated, можно использовать его;
                // иначе загрузим одного родителя (легковесный запрос)
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

            // children (immediate)
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

            // full parents chain: от ближайшего вверх до корня.
            // Мы хотим вернуть от корня -> ... -> nearest parent, поэтому соберём и затем развернём.
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

                // идём на уровень выше
                curParentId = p.ParentCategoryId;
            }

            // сейчас parentsStack = [ nearestParent, parentOfParent, ..., root ] — перевернём, чтобы root→...→nearestParent
            parentsStack.Reverse();
            foreach (var parentDto in parentsStack)
                dto.Parents.Add(parentDto);

            // return
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
}