using AutoFixture;
using FluentAssertions;
using Moq;
using ProductManager.Application.Common.Services;
using ProductManager.Application.Feature.Product.Queries;
using ProductManager.Domain.Entities;
using System.Globalization;
using System.Text;
using CsvHelper;
using Xunit;

namespace ProductManager.UnitTests.Application.Product;

public class ExportProductsHandlerTests
{
    private readonly Mock<ICrudService<Products>> _crudServiceMock;
    private readonly ExportProductsHandler _handler;
    private readonly Fixture _fixture;

    public ExportProductsHandlerTests()
    {
        _crudServiceMock = new Mock<ICrudService<Products>>();
        _handler = new ExportProductsHandler(_crudServiceMock.Object);
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task HandleAsync_WhenProductsExist_ShouldReturnCsvBytes()
    {
        // Arrange
        var products = _fixture.CreateMany<Products>(3).ToList();
        var query = new ExportProducts();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        // Convert bytes to string and verify CSV content
        var csvString = Encoding.UTF8.GetString(result);
        var lines = csvString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Length.Should().BeGreaterThan(1); // Header + data rows

        // Check that product names are in the CSV
        foreach (var product in products)
        {
            csvString.Should().Contain(product.ProductName);
        }

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenNoProductsExist_ShouldReturnEmptyCsv()
    {
        // Arrange
        var products = new List<Products>();
        var query = new ExportProducts();

        _crudServiceMock.Setup(x => x.GetAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        // Convert bytes to string and verify CSV content
        var csvString = Encoding.UTF8.GetString(result);
        var lines = csvString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Length.Should().Be(1); // Only header

        _crudServiceMock.Verify(x => x.GetAsync(), Times.Once);
    }
}
