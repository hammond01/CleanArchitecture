using Mapster;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.CategoryDto;
using ProductManager.Shared.DTOs.OrderDto;
using ProductManager.Shared.DTOs.ProductDto;
using ProductManager.Shared.DTOs.ShipperDto;
using ProductManager.Shared.DTOs.SupplierDto;
namespace ProductManager.Application.Decorators;

public static class MappingConfig
{
    public static void ConfigureMappings()
    {
        // Category mappings
        TypeAdapterConfig<Categories, GetCategoryDto>.NewConfig();
        TypeAdapterConfig<CreateCategoryDto, Categories>.NewConfig();
        TypeAdapterConfig<UpdateCategoryDto, Categories>.NewConfig();

        // Supplier mappings
        TypeAdapterConfig<Suppliers, GetSupplierDto>.NewConfig();
        TypeAdapterConfig<CreateSupplierDto, Suppliers>.NewConfig();
        TypeAdapterConfig<UpdateSupplierDto, Suppliers>.NewConfig();

        // Shipper mappings
        TypeAdapterConfig<Shippers, GetShipperDto>.NewConfig();
        TypeAdapterConfig<CreateShipperDto, Shippers>.NewConfig();
        TypeAdapterConfig<UpdateShipperDto, Shippers>.NewConfig();

        // Order mappings
        TypeAdapterConfig<Order, GetOrderDto>.NewConfig();
        TypeAdapterConfig<CreateOrderDto, Order>.NewConfig();
        TypeAdapterConfig<UpdateOrderDto, Order>.NewConfig();

        // Product mappings with custom logic
        TypeAdapterConfig<Products, GetProductDto>.NewConfig()
            .Map(member: dest => dest.CategoryName, source: src => src.Category.CategoryName)
            .Map(member: dest => dest.SupplierName, source: src => src.Supplier.CompanyName);

        TypeAdapterConfig<CreateProductDto, Products>.NewConfig();
        TypeAdapterConfig<UpdateProductDto, Products>.NewConfig();
    }
}
