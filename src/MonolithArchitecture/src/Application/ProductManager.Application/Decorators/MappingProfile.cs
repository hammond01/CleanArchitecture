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
            .ForMember(destinationMember: dest => dest.CategoryName,
            memberOptions: opt => opt.MapFrom(src => src.Category.CategoryName))
            .ForMember(destinationMember: dest => dest.SupplierName,
            memberOptions: opt => opt.MapFrom(src => src.Supplier.CompanyName));
        CreateMap<CreateProductDto, Products>().ReverseMap();
        CreateMap<UpdateProductDto, Products>().ReverseMap();
    }
}
