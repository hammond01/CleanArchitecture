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

        CreateMap<GetProductDto, Products>().ReverseMap()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.CategoryName))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier!.CompanyName));
        CreateMap<CreateProductDto, Products>().ReverseMap();
        CreateMap<UpdateProductDto, Products>().ReverseMap();
    }
}
