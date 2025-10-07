using Mapster;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.CategoryDto;
using ProductManager.Shared.DTOs.ProductDto;

namespace ProductManager.Application.Decorators;

public static class MappingConfig
{
    public static void ConfigureMappings()
    {
        // Category mappings
        TypeAdapterConfig<Categories, GetCategoryDto>.NewConfig();
        TypeAdapterConfig<CreateCategoryDto, Categories>.NewConfig();
        TypeAdapterConfig<UpdateCategoryDto, Categories>.NewConfig();

        // Product mappings with custom logic
        TypeAdapterConfig<Products, GetProductDto>
            .NewConfig()
            .Map(member: dest => dest.CategoryName, source: src => src.Category.CategoryName);

        TypeAdapterConfig<CreateProductDto, Products>.NewConfig();
        TypeAdapterConfig<UpdateProductDto, Products>.NewConfig();
    }
}
