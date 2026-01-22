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
/// 庫存調撥服務測試
/// </summary>
public class StockTransferServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly StockTransferService _service;
    private readonly Mock<ILogger<StockTransferService>> _loggerMock;

    public StockTransferServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<StockTransferService>>();
        _service = new StockTransferService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllTransfers()
    {
        // Arrange
        var transfer = new StockTransfer
        {
            TransferNumber = "TR001",
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            Status = StockTransferStatus.Pending,
            CreatedBy = 1
        };
        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchAsync(null, null, null, null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task CreateAsync_CreatesTransferWithItems()
    {
        // Arrange
        var transfer = new StockTransfer
        {
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            Status = StockTransferStatus.Pending,
            Items = new List<StockTransferItem>
            {
                new StockTransferItem { ProductId = 1, Quantity = 10 }
            }
        };

        // Act
        var result = await _service.CreateAsync(transfer, 1);

        // Assert
        result.Should().NotBeNull();
        result.TransferNumber.Should().NotBeNullOrEmpty();
        result.Status.Should().Be(StockTransferStatus.Pending);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTransferWithItems()
    {
        // Arrange
        var transfer = new StockTransfer
        {
            TransferNumber = "TR002",
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            Status = StockTransferStatus.Pending,
            CreatedBy = 1,
            Items = new List<StockTransferItem>
            {
                new StockTransferItem { ProductId = 1, Quantity = 5 }
            }
        };
        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(transfer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TransferNumber.Should().Be("TR002");
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CancelAsync_CancelsPendingTransfer()
    {
        // Arrange
        var transfer = new StockTransfer
        {
            TransferNumber = "TR003",
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            Status = StockTransferStatus.Pending,
            CreatedBy = 1
        };
        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CancelAsync(transfer.Id, 1);

        // Assert
        result.Should().BeTrue();
        var updated = await _service.GetByIdAsync(transfer.Id);
        updated!.Status.Should().Be(StockTransferStatus.Cancelled);
    }

    [Fact]
    public async Task CreateAsync_GeneratesUniqueTransferNumber()
    {
        // Arrange
        var transfer1 = new StockTransfer
        {
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            Items = new List<StockTransferItem>
            {
                new StockTransferItem { ProductId = 1, Quantity = 5 }
            }
        };
        var transfer2 = new StockTransfer
        {
            FromWarehouseId = 1,
            ToWarehouseId = 2,
            Items = new List<StockTransferItem>
            {
                new StockTransferItem { ProductId = 1, Quantity = 3 }
            }
        };

        // Act
        var result1 = await _service.CreateAsync(transfer1, 1);
        var result2 = await _service.CreateAsync(transfer2, 1);

        // Assert
        result1.TransferNumber.Should().NotBeNullOrEmpty();
        result2.TransferNumber.Should().NotBeNullOrEmpty();
        result1.TransferNumber.Should().NotBe(result2.TransferNumber);
        result1.TransferNumber.Should().StartWith("TRF");
        result2.TransferNumber.Should().StartWith("TRF");
    }
}
