using AutoMapper;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Mappings;

/// <summary>
/// AutoMapper profile for ProductCatalog mappings
/// </summary>
public class ProductCatalogMappingProfile : Profile
{
    public ProductCatalogMappingProfile()
    {
        CreateProductMappings();
        CreateCategoryMappings();
        CreateSupplierMappings();
    }

    private void CreateProductMappings()
    {
        // Product entity to ProductDto
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.CompanyName : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock()))
            .ForMember(dest => dest.IsOutOfStock, opt => opt.MapFrom(src => src.IsOutOfStock()))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable()));

        // Product entity to ProductSummaryDto
        CreateMap<Product, ProductSummaryDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.CompanyName : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock()))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable()));
    }

    private void CreateCategoryMappings()
    {
        // Category entity to CategoryDto
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

        // Category entity to CategorySummaryDto
        CreateMap<Category, CategorySummaryDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
    }

    private void CreateSupplierMappings()
    {
        // Supplier entity to SupplierDto
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating.ToString()))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

        // Supplier entity to SupplierSummaryDto
        CreateMap<Supplier, SupplierSummaryDto>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating.ToString()))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
    }
}
