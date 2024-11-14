using ProductManager.Shared.DTOs.OrderDto;
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

        CreateMap<GetOrderDto, Order>().ReverseMap();
        CreateMap<CreateOrderDto, Order>().ReverseMap();
        CreateMap<UpdateOrderDto, Order>().ReverseMap();
    }
}
