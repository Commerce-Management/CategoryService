using AutoMapper;
using CategoryService.Core.Entities;
using CategoryService.Shared.Dtos;

namespace CategoryService.Core.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CreateCategoryDto,Category>();
        CreateMap<Category, GetCategoryDto>();
    }
}