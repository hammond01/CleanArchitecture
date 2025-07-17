using Xunit;
using ProductCatalog.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace UnitTests.ProductCatalog.Controllers;

public class ProductsControllerTests
{
    [Fact]
    public void GetLowStockProducts_ReturnsOkResult_WithExpectedProducts()
    {
        // Arrange
        var controller = new ProductsController();

        // Act
        var result = controller.GetLowStockProducts() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var products = result.Value as IEnumerable<object>;
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }
}
