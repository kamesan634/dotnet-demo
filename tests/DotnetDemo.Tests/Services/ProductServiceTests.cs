using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Implementations;
using DotnetDemo.Tests.TestHelpers;
using Xunit;

namespace DotnetDemo.Tests.Services;

/// <summary>
/// 商品服務測試
/// </summary>
public class ProductServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductService _service;
    private readonly Mock<ILogger<ProductService>> _loggerMock;

    public ProductServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task SearchAsync_WithSearchText_ReturnsMatchingProducts()
    {
        // Act
        var result = await _service.SearchAsync("測試商品1", null, null);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("測試商品1");
    }

    [Fact]
    public async Task SearchAsync_WithBarcode_ReturnsMatchingProducts()
    {
        // Act
        var result = await _service.SearchAsync("1234567890", null, null);

        // Assert
        result.Should().ContainSingle();
        result[0].Barcode.Should().Be("1234567890");
    }

    [Fact]
    public async Task SearchAsync_WithCategoryId_ReturnsFilteredProducts()
    {
        // Act
        var result = await _service.SearchAsync(null, 1, null);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().OnlyContain(p => p.CategoryId == 1);
    }

    [Fact]
    public async Task SearchAsync_WithIsActive_ReturnsFilteredProducts()
    {
        // Act
        var activeResult = await _service.SearchAsync(null, null, true);
        var inactiveResult = await _service.SearchAsync(null, null, false);

        // Assert
        activeResult.Should().OnlyContain(p => p.IsActive);
        inactiveResult.Should().OnlyContain(p => !p.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Category.Should().NotBeNull();
        result.Unit.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _service.GetByIdAsync(99999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var product = new Product
        {
            Code = "P999",
            Name = "新商品",
            CategoryId = 1,
            UnitId = 1,
            CostPrice = 50,
            SellingPrice = 100,
            SafetyStock = 10,
            IsActive = true
        };

        // Act
        var result = await _service.CreateAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        var product = await _service.GetByIdAsync(1);
        product!.Name = "更新後的名稱";

        // Act
        var result = await _service.UpdateAsync(product);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByIdAsync(1);
        updated!.Name.Should().Be("更新後的名稱");
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingProduct_ReturnsFalse()
    {
        // Arrange
        var product = new Product { Id = 99999, Code = "XXX", Name = "不存在" };

        // Act
        var result = await _service.UpdateAsync(product);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        var newProduct = await _service.CreateAsync(new Product
        {
            Code = "DEL001",
            Name = "待刪除商品",
            CategoryId = 1,
            UnitId = 1,
            CostPrice = 10,
            SellingPrice = 20,
            SafetyStock = 5,
            IsActive = true
        });

        // Act
        var result = await _service.DeleteAsync(newProduct.Id);

        // Assert
        result.Should().BeTrue();

        var deleted = await _service.GetByIdAsync(newProduct.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingProduct_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteAsync(99999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CodeExistsAsync_ExistingCode_ReturnsTrue()
    {
        // Act
        var result = await _service.CodeExistsAsync("P001");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CodeExistsAsync_NonExistingCode_ReturnsFalse()
    {
        // Act
        var result = await _service.CodeExistsAsync("NOTEXIST");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CodeExistsAsync_WithExcludeId_ExcludesProduct()
    {
        // Act
        var result = await _service.CodeExistsAsync("P001", excludeId: 1);

        // Assert
        result.Should().BeFalse();
    }
}
