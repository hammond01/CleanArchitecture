using AutoFixture;
using FluentAssertions;
using ProductManager.Application.Feature.Product.Commands;
using ProductManager.Application.Feature.Product.Queries;
using ProductManager.Domain.Entities;
using Xunit;

namespace ProductManager.UnitTests.Application.Product;

public class ProductCommandQueryTests
{
    private readonly Fixture _fixture;

    public ProductCommandQueryTests()
    {
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    #region AddOrUpdateProductCommand Tests

    [Fact]
    public void AddOrUpdateProductCommand_Constructor_ShouldSetProductProperty()
    {
        // Arrange
        var product = _fixture.Create<Products>();

        // Act
        var command = new AddOrUpdateProductCommand(product);

        // Assert
        command.Products.Should().BeEquivalentTo(product);
        command.Products.Should().BeSameAs(product);
    }

    [Fact]
    public void AddOrUpdateProductCommand_WithNullProduct_ShouldAllowNullValue()
    {
        // Act
        var command = new AddOrUpdateProductCommand(null!);

        // Assert
        command.Products.Should().BeNull();
    }

    [Fact]
    public void AddOrUpdateProductCommand_ProductProperty_ShouldBeSettable()
    {
        // Arrange
        var initialProduct = _fixture.Create<Products>();
        var newProduct = _fixture.Create<Products>();
        var command = new AddOrUpdateProductCommand(initialProduct);

        // Act
        command.Products = newProduct;

        // Assert
        command.Products.Should().BeEquivalentTo(newProduct);
        command.Products.Should().BeSameAs(newProduct);
    }

    #endregion

    #region DeleteProductCommand Tests

    [Fact]
    public void DeleteProductCommand_Constructor_ShouldSetProductProperty()
    {
        // Arrange
        var product = _fixture.Create<Products>();

        // Act
        var command = new DeleteProductCommand(product);

        // Assert
        command.Product.Should().BeEquivalentTo(product);
        command.Product.Should().BeSameAs(product);
    }

    [Fact]
    public void DeleteProductCommand_WithNullProduct_ShouldAllowNullValue()
    {
        // Act
        var command = new DeleteProductCommand(null!);

        // Assert
        command.Product.Should().BeNull();
    }

    [Fact]
    public void DeleteProductCommand_ProductProperty_ShouldBeSettable()
    {
        // Arrange
        var initialProduct = _fixture.Create<Products>();
        var newProduct = _fixture.Create<Products>();
        var command = new DeleteProductCommand(initialProduct);

        // Act
        command.Product = newProduct;

        // Assert
        command.Product.Should().BeEquivalentTo(newProduct);
        command.Product.Should().BeSameAs(newProduct);
    }

    #endregion

    #region GetProducts Tests

    [Fact]
    public void GetProducts_ShouldBeRecord()
    {
        // Act
        var query1 = new GetProducts();
        var query2 = new GetProducts();

        // Assert - Records should be equal and have same hash code
        query1.Equals(query2).Should().BeTrue();
        query1.GetHashCode().Should().Be(query2.GetHashCode());
        (query1 == query2).Should().BeTrue();
    }

    [Fact]
    public void GetProducts_ShouldImplementIQuery()
    {
        // Act
        var query = new GetProducts();

        // Assert
        query.Should().BeAssignableTo<ProductManager.Application.Common.Queries.IQuery<ProductManager.Domain.Common.ApiResponse>>();
    }

    #endregion

    #region GetProductByIdQuery Tests

    [Fact]
    public void GetProductByIdQuery_Constructor_ShouldSetProductIdProperty()
    {
        // Arrange
        var productId = "test-product-id";

        // Act
        var query = new GetProductByIdQuery(productId);

        // Assert
        query.ProductId.Should().Be(productId);
    }

    [Fact]
    public void GetProductByIdQuery_WithNullProductId_ShouldAllowNullValue()
    {
        // Act
        var query = new GetProductByIdQuery(null!);

        // Assert
        query.ProductId.Should().BeNull();
    }

    [Fact]
    public void GetProductByIdQuery_WithEmptyProductId_ShouldAllowEmptyValue()
    {
        // Act
        var query = new GetProductByIdQuery(string.Empty);

        // Assert
        query.ProductId.Should().BeEmpty();
    }

    [Fact]
    public void GetProductByIdQuery_ProductIdProperty_ShouldBeSettable()
    {
        // Arrange
        var initialId = "initial-id";
        var newId = "new-id";
        var query = new GetProductByIdQuery(initialId);

        // Act
        query.ProductId = newId;

        // Assert
        query.ProductId.Should().Be(newId);
    }

    [Fact]
    public void GetProductByIdQuery_ShouldBeRecord()
    {
        // Arrange
        var productId = "same-id";

        // Act
        var query1 = new GetProductByIdQuery(productId);
        var query2 = new GetProductByIdQuery(productId);

        // Assert
        query1.Should().BeEquivalentTo(query2);
        query1.Equals(query2).Should().BeTrue();
    }

    [Fact]
    public void GetProductByIdQuery_WithDifferentIds_ShouldNotBeEqual()
    {
        // Act
        var query1 = new GetProductByIdQuery("id1");
        var query2 = new GetProductByIdQuery("id2");

        // Assert
        query1.Should().NotBeEquivalentTo(query2);
        query1.Equals(query2).Should().BeFalse();
    }

    [Theory]
    [InlineData("valid-id")]
    [InlineData("another-id")]
    [InlineData("123")]
    [InlineData("special-chars-!@#")]
    public void GetProductByIdQuery_WithVariousValidIds_ShouldSetCorrectly(string productId)
    {
        // Act
        var query = new GetProductByIdQuery(productId);

        // Assert
        query.ProductId.Should().Be(productId);
    }

    #endregion

    #region Command/Query Interface Implementation Tests

    [Fact]
    public void AddOrUpdateProductCommand_ShouldImplementICommand()
    {
        // Arrange
        var product = _fixture.Create<Products>();

        // Act
        var command = new AddOrUpdateProductCommand(product);

        // Assert
        command.Should().BeAssignableTo<ProductManager.Application.Common.Commands.ICommand<ProductManager.Domain.Common.ApiResponse>>();
    }

    [Fact]
    public void DeleteProductCommand_ShouldImplementICommand()
    {
        // Arrange
        var product = _fixture.Create<Products>();

        // Act
        var command = new DeleteProductCommand(product);

        // Assert
        command.Should().BeAssignableTo<ProductManager.Application.Common.Commands.ICommand<ProductManager.Domain.Common.ApiResponse>>();
    }

    [Fact]
    public void GetProductByIdQuery_ShouldImplementIQuery()
    {
        // Act
        var query = new GetProductByIdQuery("test-id");

        // Assert
        query.Should().BeAssignableTo<ProductManager.Application.Common.Queries.IQuery<ProductManager.Domain.Common.ApiResponse>>();
    }

    #endregion
}
