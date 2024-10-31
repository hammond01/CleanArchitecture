using ProductManager.Shared.DTOs.CategoryDto;
namespace ProductManager.Application.Decorators;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GetCategoryDto, Categories>().ReverseMap();
        CreateMap<CreateCategoryDto, Categories>().ReverseMap();
    }
}
