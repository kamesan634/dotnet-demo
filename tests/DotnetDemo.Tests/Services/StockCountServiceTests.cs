using FluentAssertions;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Implementations;
using DotnetDemo.Tests.TestHelpers;
using Xunit;

namespace DotnetDemo.Tests.Services;

/// <summary>
/// 庫存盤點服務測試
/// </summary>
public class StockCountServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly StockCountService _service;

    public StockCountServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _service = new StockCountService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllStockCounts()
    {
        // Arrange
        var stockCount = new StockCount
        {
            CountNumber = "SC001",
            WarehouseId = 1,
            CountDate = DateTime.Today,
            Status = StockCountStatus.Draft,
            CreatedBy = 1
        };
        _context.StockCounts.Add(stockCount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchAsync(null, null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task CreateAsync_CreatesStockCountSuccessfully()
    {
        // Arrange
        var stockCount = new StockCount
        {
            WarehouseId = 1,
            CountDate = DateTime.Today,
            Scope = "全部商品"
        };

        // Act
        var result = await _service.CreateAsync(stockCount, 1);

        // Assert
        result.Should().NotBeNull();
        result.CountNumber.Should().NotBeNullOrEmpty();
        result.Status.Should().Be(StockCountStatus.Draft);
        result.CreatedBy.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsStockCountWithItems()
    {
        // Arrange
        var stockCount = new StockCount
        {
            CountNumber = "SC002",
            WarehouseId = 1,
            CountDate = DateTime.Today,
            Status = StockCountStatus.InProgress,
            CreatedBy = 1,
            Items = new List<StockCountItem>
            {
                new StockCountItem
                {
                    ProductId = 1,
                    SystemQuantity = 100,
                    IsCounted = false
                }
            }
        };
        _context.StockCounts.Add(stockCount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(stockCount.Id);

        // Assert
        result.Should().NotBeNull();
        result!.CountNumber.Should().Be("SC002");
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_UpdatesItemCorrectly()
    {
        // Arrange
        var stockCount = new StockCount
        {
            CountNumber = "SC003",
            WarehouseId = 1,
            CountDate = DateTime.Today,
            Status = StockCountStatus.InProgress,
            CreatedBy = 1,
            Items = new List<StockCountItem>
            {
                new StockCountItem
                {
                    ProductId = 1,
                    SystemQuantity = 100,
                    IsCounted = false
                }
            }
        };
        _context.StockCounts.Add(stockCount);
        await _context.SaveChangesAsync();

        var item = stockCount.Items.First();

        // Act
        var result = await _service.UpdateItemQuantityAsync(item.Id, 95, 1, "短少5個");

        // Assert
        result.Should().BeTrue();
        var updatedItem = _context.StockCountItems.Find(item.Id);
        updatedItem!.ActualQuantity.Should().Be(95);
        updatedItem.Difference.Should().Be(-5);
        updatedItem.IsCounted.Should().BeTrue();
        updatedItem.DifferenceReason.Should().Be("短少5個");
    }

    [Fact]
    public async Task CancelAsync_CancelsStockCount()
    {
        // Arrange
        var stockCount = new StockCount
        {
            CountNumber = "SC004",
            WarehouseId = 1,
            CountDate = DateTime.Today,
            Status = StockCountStatus.Draft,
            CreatedBy = 1
        };
        _context.StockCounts.Add(stockCount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CancelAsync(stockCount.Id, 1);

        // Assert
        result.Should().BeTrue();
        var cancelled = await _service.GetByIdAsync(stockCount.Id);
        cancelled!.Status.Should().Be(StockCountStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_CannotCancelCompletedStockCount()
    {
        // Arrange
        var stockCount = new StockCount
        {
            CountNumber = "SC005",
            WarehouseId = 1,
            CountDate = DateTime.Today,
            Status = StockCountStatus.Completed,
            CreatedBy = 1
        };
        _context.StockCounts.Add(stockCount);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CancelAsync(stockCount.Id, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GenerateCountNumberAsync_ReturnsUniqueNumber()
    {
        // Act
        var number = await _service.GenerateCountNumberAsync();

        // Assert
        number.Should().NotBeNullOrEmpty();
        number.Should().StartWith("SC");
    }
}
