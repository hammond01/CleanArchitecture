using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Mappings;

/// <summary>
/// Product mapping extensions - thay thế AutoMapper
/// </summary>
public static class ProductMappingExtensions
{
    /// <summary>
    /// Convert Product entity to ProductDto
    /// </summary>
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.CategoryName,
            SupplierId = product.SupplierId,
            SupplierName = product.Supplier?.CompanyName,
            QuantityPerUnit = product.QuantityPerUnit,
            UnitPrice = product.UnitPrice,
            UnitsInStock = product.UnitsInStock,
            UnitsOnOrder = product.UnitsOnOrder,
            ReorderLevel = product.ReorderLevel,
            Discontinued = product.Discontinued,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    /// <summary>
    /// Convert CreateProductDto to Product entity
    /// </summary>
    public static Product ToEntity(this CreateProductDto dto, string productId)
    {
        return new Product(
            productId,
            dto.ProductName,
            dto.CategoryId,
            dto.SupplierId,
            dto.QuantityPerUnit,
            dto.UnitPrice,
            dto.UnitsInStock,
            dto.UnitsOnOrder,
            dto.ReorderLevel,
            dto.Discontinued
        );
    }

    /// <summary>
    /// Update Product entity from UpdateProductDto
    /// </summary>
    public static void UpdateFromDto(this Product product, UpdateProductDto dto)
    {
        product.UpdateProduct(
            dto.ProductName,
            dto.CategoryId,
            dto.SupplierId,
            dto.QuantityPerUnit,
            dto.UnitPrice,
            dto.UnitsInStock,
            dto.UnitsOnOrder,
            dto.ReorderLevel,
            dto.Discontinued
        );
    }

    /// <summary>
    /// Convert collection of Products to ProductDtos
    /// </summary>
    public static IEnumerable<ProductDto> ToDto(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToDto());
    }
}

/// <summary>
/// Category mapping extensions
/// </summary>
public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            PictureLink = category.PictureLink,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public static IEnumerable<CategoryDto> ToDto(this IEnumerable<Category> categories)
    {
        return categories.Select(c => c.ToDto());
    }
}

/// <summary>
/// Supplier mapping extensions
/// </summary>
public static class SupplierMappingExtensions
{
    public static SupplierDto ToDto(this Supplier supplier)
    {
        return new SupplierDto
        {
            SupplierId = supplier.SupplierId,
            CompanyName = supplier.CompanyName,
            ContactName = supplier.ContactName,
            ContactTitle = supplier.ContactTitle,
            Address = supplier.Address,
            City = supplier.City,
            Region = supplier.Region,
            PostalCode = supplier.PostalCode,
            Country = supplier.Country,
            Phone = supplier.Phone,
            Fax = supplier.Fax,
            HomePage = supplier.HomePage,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };
    }

    public static IEnumerable<SupplierDto> ToDto(this IEnumerable<Supplier> suppliers)
    {
        return suppliers.Select(s => s.ToDto());
    }
}
