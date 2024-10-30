using ProductManager.Shared.DTOs.CategoryDto;
namespace ProductManager.Application.Decorators;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GetCategoryDto, Category>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>().ReverseMap();
    }
}
