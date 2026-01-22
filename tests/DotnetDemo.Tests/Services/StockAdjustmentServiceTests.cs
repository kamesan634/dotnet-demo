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
/// 庫存調整服務測試
/// </summary>
public class StockAdjustmentServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly StockAdjustmentService _service;
    private readonly Mock<ILogger<StockAdjustmentService>> _loggerMock;

    public StockAdjustmentServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<StockAdjustmentService>>();
        _service = new StockAdjustmentService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllAdjustments()
    {
        // Arrange
        var adjustment = new StockAdjustment
        {
            AdjustmentNumber = "SA001",
            WarehouseId = 1,
            Reason = "盤點調整",
            CreatedBy = 1
        };
        _context.StockAdjustments.Add(adjustment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchAsync(null, null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task CreateAsync_CreatesAdjustmentAndUpdatesInventory()
    {
        // Arrange
        var inventory = _context.Inventories.First();
        var originalQuantity = inventory.Quantity;
        var newQuantity = originalQuantity + 10;

        var adjustment = new StockAdjustment
        {
            WarehouseId = inventory.WarehouseId,
            Reason = "測試調整",
            Items = new List<StockAdjustmentItem>
            {
                new StockAdjustmentItem
                {
                    ProductId = inventory.ProductId,
                    BeforeQuantity = originalQuantity,
                    AfterQuantity = newQuantity
                }
            }
        };

        // Act
        var result = await _service.CreateAsync(adjustment, 1);

        // Assert
        result.Should().NotBeNull();
        result.AdjustmentNumber.Should().NotBeNullOrEmpty();

        var updatedInventory = _context.Inventories.First(i => i.Id == inventory.Id);
        updatedInventory.Quantity.Should().Be(newQuantity);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAdjustmentWithItems()
    {
        // Arrange
        var adjustment = new StockAdjustment
        {
            AdjustmentNumber = "SA002",
            WarehouseId = 1,
            Reason = "盤點差異",
            CreatedBy = 1,
            Items = new List<StockAdjustmentItem>
            {
                new StockAdjustmentItem
                {
                    ProductId = 1,
                    BeforeQuantity = 100,
                    AfterQuantity = 95,
                    AdjustmentQuantity = -5
                }
            }
        };
        _context.StockAdjustments.Add(adjustment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(adjustment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.AdjustmentNumber.Should().Be("SA002");
        result.Reason.Should().Be("盤點差異");
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_GeneratesUniqueAdjustmentNumber()
    {
        // Arrange
        var inventory = _context.Inventories.First();

        var adjustment1 = new StockAdjustment
        {
            WarehouseId = inventory.WarehouseId,
            Reason = "測試調整1",
            Items = new List<StockAdjustmentItem>
            {
                new StockAdjustmentItem
                {
                    ProductId = inventory.ProductId,
                    BeforeQuantity = inventory.Quantity,
                    AfterQuantity = inventory.Quantity + 5
                }
            }
        };

        var adjustment2 = new StockAdjustment
        {
            WarehouseId = inventory.WarehouseId,
            Reason = "測試調整2",
            Items = new List<StockAdjustmentItem>
            {
                new StockAdjustmentItem
                {
                    ProductId = inventory.ProductId,
                    BeforeQuantity = inventory.Quantity + 5,
                    AfterQuantity = inventory.Quantity + 10
                }
            }
        };

        // Act
        var result1 = await _service.CreateAsync(adjustment1, 1);
        var result2 = await _service.CreateAsync(adjustment2, 1);

        // Assert
        result1.AdjustmentNumber.Should().NotBeNullOrEmpty();
        result2.AdjustmentNumber.Should().NotBeNullOrEmpty();
        result1.AdjustmentNumber.Should().NotBe(result2.AdjustmentNumber);
        result1.AdjustmentNumber.Should().StartWith("ADJ");
        result2.AdjustmentNumber.Should().StartWith("ADJ");
    }
}
