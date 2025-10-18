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
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(d => d.ImageUrl, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

    }
}