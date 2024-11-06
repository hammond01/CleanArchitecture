using ProductManager.Shared.DTOs.SupplierDto;
namespace ProductManager.Application.Decorators;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GetCategoryDto, Categories>().ReverseMap();
        CreateMap<CreateCategoryDto, Categories>().ReverseMap();
        CreateMap<UpdateCategoryDto, Categories>().ReverseMap();

        CreateMap<GetSupplierDto, Suppliers>().ReverseMap();
        CreateMap<CreateSupplierDto, Suppliers>().ReverseMap();
        CreateMap<UpdateSupplierDto, Suppliers>().ReverseMap();
    }
}
