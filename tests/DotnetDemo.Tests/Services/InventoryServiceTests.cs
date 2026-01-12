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
/// 庫存服務測試
/// </summary>
public class InventoryServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly InventoryService _service;
    private readonly Mock<ILogger<InventoryService>> _loggerMock;

    public InventoryServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<InventoryService>>();
        _service = new InventoryService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllInventories()
    {
        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task SearchAsync_WithSearchText_ReturnsMatchingInventories()
    {
        // Act
        var result = await _service.SearchAsync("測試商品1", null, false);

        // Assert
        result.Should().ContainSingle();
        result[0].Product.Name.Should().Be("測試商品1");
    }

    [Fact]
    public async Task SearchAsync_WithWarehouseId_ReturnsFilteredInventories()
    {
        // Act
        var result = await _service.SearchAsync(null, 1, false);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().OnlyContain(i => i.WarehouseId == 1);
    }

    [Fact]
    public async Task SearchAsync_LowStockOnly_ReturnsLowStockItems()
    {
        // Product 2 has quantity 3, safety stock 5

        // Act
        var result = await _service.SearchAsync(null, null, true);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().OnlyContain(i => i.Quantity < i.Product.SafetyStock);
    }

    [Fact]
    public async Task GetLowStockAsync_ReturnsLowStockItems()
    {
        // Act
        var result = await _service.GetLowStockAsync();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().OnlyContain(i => i.Quantity < i.Product.SafetyStock);
    }

    [Fact]
    public async Task GetByProductAndWarehouseAsync_Existing_ReturnsInventory()
    {
        // Act
        var result = await _service.GetByProductAndWarehouseAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result!.ProductId.Should().Be(1);
        result.WarehouseId.Should().Be(1);
    }

    [Fact]
    public async Task GetByProductAndWarehouseAsync_NonExisting_ReturnsNull()
    {
        // Act
        var result = await _service.GetByProductAndWarehouseAsync(99999, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AdjustQuantityAsync_ExistingInventory_AdjustsQuantity()
    {
        // Arrange
        var originalInventory = await _service.GetByProductAndWarehouseAsync(1, 1);
        var originalQuantity = originalInventory!.Quantity;

        // Act
        var result = await _service.AdjustQuantityAsync(1, 1, 10, "測試調整", 1);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByProductAndWarehouseAsync(1, 1);
        updated!.Quantity.Should().Be(originalQuantity + 10);
    }

    [Fact]
    public async Task AdjustQuantityAsync_NewInventory_CreatesInventory()
    {
        // Arrange - Product 3 has no inventory in warehouse 1

        // Act
        var result = await _service.AdjustQuantityAsync(3, 1, 50, "新增庫存", 1);

        // Assert
        result.Should().BeTrue();

        var created = await _service.GetByProductAndWarehouseAsync(3, 1);
        created.Should().NotBeNull();
        created!.Quantity.Should().Be(50);
    }

    [Fact]
    public async Task AdjustQuantityAsync_CreatesMovementRecord()
    {
        // Act
        await _service.AdjustQuantityAsync(1, 1, 5, "測試異動", 1);

        // Assert
        var movements = await _service.GetMovementsAsync(1, 1, null, null);
        movements.Should().Contain(m => m.Notes == "測試異動" && m.Quantity == 5);
    }

    [Fact]
    public async Task GetMovementsAsync_ReturnsMovements()
    {
        // Arrange
        await _service.AdjustQuantityAsync(1, 1, 10, "異動1", 1);
        await _service.AdjustQuantityAsync(1, 1, -5, "異動2", 1);

        // Act
        var result = await _service.GetMovementsAsync(1, null, null, null);

        // Assert
        result.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetMovementsAsync_WithDateFilter_ReturnsFilteredMovements()
    {
        // Arrange
        await _service.AdjustQuantityAsync(1, 1, 10, "今天的異動", 1);

        // Act
        var result = await _service.GetMovementsAsync(1, null, DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1));

        // Assert
        result.Should().NotBeEmpty();
    }
}
