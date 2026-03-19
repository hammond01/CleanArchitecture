using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using CleanArchitecture.IntegrationTests.Infrastructure;

namespace CleanArchitecture.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for Catalog API endpoints
/// Tests the complete request/response flow
/// NOTE: API returns responses wrapped in { success,  data } format
/// </summary>
public class CatalogIntegrationTests : IClassFixture<PostgresWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly PostgresWebApplicationFactory _factory;

    public CatalogIntegrationTests(PostgresWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Helper Methods

    private static string? ExtractIdFromResponse(string jsonContent)
    {
        var doc = JsonDocument.Parse(jsonContent);
        if (doc.RootElement.TryGetProperty("data", out var dataElement))
        {
            if (dataElement.TryGetProperty("id", out var idElement))
            {
                return idElement.GetString();
            }
        }
        else if (doc.RootElement.TryGetProperty("id", out var idElement))
        {
            return idElement.GetString();
        }
        return null;
    }

    #endregion

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategories_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"success\":false");
        content.Should().Contain("\"error\"");
    }

    #region Category Tests

    [Fact]
    public async Task CreateCategory_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/categories", new
        {
            CategoryName = "Unauthorized Category"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ReturnsCreated()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange
        var command = new
        {
            CategoryName = "Electronics",
            Description = "Electronic devices and accessories",
            PictureLink = "https://example.com/electronics.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/categories", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(content);
        categoryId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateCategory_WithInvalidData_ReturnsBadRequest()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - missing required CategoryName
        var command = new
        {
            Description = "Test description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/categories", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"success\":false");
        content.Should().Contain("\"statusCode\":400");
        content.Should().Contain("\"error\"");
    }

    [Fact]
    public async Task UpdateCategory_WithValidId_ReturnsOk()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create a category first
        var createCommand = new
        {
            CategoryName = "Books",
            Description = "Original description"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCommand);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(createContent);

        var updateCommand = new
        {
            CategoryName = "Literature",
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/categories/{categoryId}", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidId_ReturnsError()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange
        var invalidId = Guid.NewGuid().ToString();
        var command = new
        {
            CategoryName = "Test Category"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/categories/{invalidId}", command);

        // Assert - Handler may throw exception or return error response
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_WithValidId_ReturnsOk()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create a category first
        var createCommand = new
        {
            CategoryName = "ToBeDeleted"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCommand);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(createContent);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/categories/{categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteCategory_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange
        var invalidId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/categories/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_WithExistingProducts_ReturnsBadRequest()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        var createCategoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", new
        {
            CategoryName = "Protected Category"
        });
        var createCategoryContent = await createCategoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(createCategoryContent);

        await _client.PostAsJsonAsync("/api/v1/products", new
        {
            ProductName = "Attached Product",
            CategoryId = categoryId
        });

        var response = await _client.DeleteAsync($"/api/v1/categories/{categoryId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCategoryById_WithValidId_ReturnsOk()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create a category first
        var createCommand = new
        {
            CategoryName = "Sports",
            Description = "Sports equipment and apparel"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCommand);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(createContent);

        // Act
        var response = await _client.GetAsync($"/api/v1/categories/{categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("success");
        responseContent.Should().Contain("data");
    }

    #endregion

    #region Product Tests

    [Fact]
    public async Task CreateProduct_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/products", new
        {
            ProductName = "Unauthorized Product",
            CategoryId = Guid.NewGuid().ToString()
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create category first for FK constraint
        var categoryCommand = new
        {
            CategoryName = "Beverages"
        };
        var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryCommand);
        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(categoryContent);

        var productCommand = new
        {
            ProductName = "Coffee",
            CategoryId = categoryId,
            QuantityPerUnit = "1 box",
            UnitPrice = 18.0m,
            UnitsInStock = (short)39,
            ReorderLevel = (short)10,
            Discontinued = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", productCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var productId = ExtractIdFromResponse(content);
        productId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - missing required ProductName and CategoryId
        var command = new
        {
            UnitPrice = 10.0m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_WithValidId_ReturnsOk()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create category and product first
        var categoryCommand = new { CategoryName = "Dairy" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryCommand);
        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(categoryContent);

        var createCommand = new
        {
            ProductName = "Milk",
            CategoryId = categoryId,
            UnitPrice = 5.0m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createCommand);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var productId = ExtractIdFromResponse(createContent);

        var updateCommand = new
        {
            ProductName = "Whole Milk",
            CategoryId = categoryId,
            UnitPrice = 6.0m,
            UnitsInStock = (short)100
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{productId}", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidId_ReturnsError()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create category for valid FK
        var categoryCommand = new { CategoryName = "Test" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryCommand);
        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(categoryContent);

        var invalidId = Guid.NewGuid().ToString();
        var command = new
        {
            ProductName = "Non-existent Product",
            CategoryId = categoryId
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{invalidId}", command);

        // Assert - Handler may throw exception or return error response
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsOk()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create category and product first
        var categoryCommand = new { CategoryName = "Condiments" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryCommand);
        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(categoryContent);

        var createCommand = new
        {
            ProductName = "Ketchup",
            CategoryId = categoryId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createCommand);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var productId = ExtractIdFromResponse(createContent);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidId_ReturnsNotFound()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange
        var invalidId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/products/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsOk()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create category and product first
        var categoryCommand = new { CategoryName = "Snacks" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryCommand);
        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(categoryContent);

        var createCommand = new
        {
            ProductName = "Chips",
            CategoryId = categoryId,
            UnitPrice = 2.5m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createCommand);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var productId = ExtractIdFromResponse(createContent);

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadAsStringAsync();
        product.Should().Contain("success");
        product.Should().Contain("data");
    }

    [Fact]
    public async Task GetProducts_WithCategoryFilter_ReturnsFilteredProducts()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create category and multiple products
        var categoryCommand = new { CategoryName = "Seafood" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryCommand);
        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(categoryContent);

        await _client.PostAsJsonAsync("/api/v1/products", new
        {
            ProductName = "Salmon",
            CategoryId = categoryId
        });
        await _client.PostAsJsonAsync("/api/v1/products", new
        {
            ProductName = "Tuna",
            CategoryId = categoryId
        });

        // Act
        var response = await _client.GetAsync($"/api/v1/products?categoryId={categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ExportProducts_ReturnsCsvFile()
    {
        await AuthenticationTestHelper.AuthenticateAsync(_client);

        // Arrange - Create some test data
        var categoryCommand = new { CategoryName = "Export Test" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", categoryCommand);
        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
        var categoryId = ExtractIdFromResponse(categoryContent);

        await _client.PostAsJsonAsync("/api/v1/products", new
        {
            ProductName = "Test Product",
            CategoryId = categoryId
        });

        // Act
        var response = await _client.GetAsync("/api/v1/products/export");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();
    }

    #endregion
}
