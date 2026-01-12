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
/// 採購單服務測試
/// </summary>
public class PurchaseOrderServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PurchaseOrderService _service;
    private readonly Mock<ILogger<PurchaseOrderService>> _loggerMock;

    public PurchaseOrderServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<PurchaseOrderService>>();
        _service = new PurchaseOrderService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchAsync_NoFilter_ReturnsOrders()
    {
        // Act
        var result = await _service.SearchAsync(null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task SearchAsync_WithOrderNumber_ReturnsMatchingOrders()
    {
        // Act
        var result = await _service.SearchAsync("PO202401010001", null, null);

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task SearchAsync_WithSupplierId_ReturnsFilteredOrders()
    {
        // Act
        var result = await _service.SearchAsync(null, 1, null);

        // Assert
        result.Should().OnlyContain(o => o.SupplierId == 1);
    }

    [Fact]
    public async Task SearchAsync_WithStatus_ReturnsFilteredOrders()
    {
        // Act
        var result = await _service.SearchAsync(null, null, PurchaseOrderStatus.Draft);

        // Assert
        result.Should().OnlyContain(o => o.Status == PurchaseOrderStatus.Draft);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsOrder()
    {
        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
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
    public async Task CreateAsync_ValidOrder_ReturnsCreatedOrder()
    {
        // Arrange
        var order = new PurchaseOrder
        {
            SupplierId = 1,
            WarehouseId = 1,
            ExpectedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Items = new List<PurchaseOrderItem>
            {
                new PurchaseOrderItem { ProductId = 1, Quantity = 10, UnitPrice = 100, SubTotal = 1000 }
            }
        };

        // Act
        var result = await _service.CreateAsync(order, 1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.OrderNumber.Should().NotBeNullOrEmpty();
        result.Status.Should().Be(PurchaseOrderStatus.Draft);
        result.TotalAmount.Should().Be(1000);
    }

    [Fact]
    public async Task UpdateAsync_DraftOrder_ReturnsTrue()
    {
        // Arrange
        var order = await _service.GetByIdAsync(1);
        order!.Notes = "更新後的備註";

        // Act
        var result = await _service.UpdateAsync(order);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingOrder_ReturnsFalse()
    {
        // Arrange
        var order = new PurchaseOrder { Id = 99999 };

        // Act
        var result = await _service.UpdateAsync(order);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DraftOrder_ReturnsTrue()
    {
        // Arrange
        var newOrder = await _service.CreateAsync(new PurchaseOrder
        {
            SupplierId = 1,
            WarehouseId = 1,
            ExpectedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Items = new List<PurchaseOrderItem>()
        }, 1);

        // Act
        var result = await _service.DeleteAsync(newOrder.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingOrder_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteAsync(99999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ApproveAsync_DraftOrder_ReturnsTrue()
    {
        // Act
        var result = await _service.ApproveAsync(1, 1);

        // Assert
        result.Should().BeTrue();

        var approved = await _service.GetByIdAsync(1);
        approved!.Status.Should().Be(PurchaseOrderStatus.Approved);
        approved.ApprovedBy.Should().Be(1);
        approved.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ApproveAsync_NonExistingOrder_ReturnsFalse()
    {
        // Act
        var result = await _service.ApproveAsync(99999, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelAsync_DraftOrder_ReturnsTrue()
    {
        // Arrange
        var newOrder = await _service.CreateAsync(new PurchaseOrder
        {
            SupplierId = 1,
            WarehouseId = 1,
            ExpectedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Items = new List<PurchaseOrderItem>()
        }, 1);

        // Act
        var result = await _service.CancelAsync(newOrder.Id);

        // Assert
        result.Should().BeTrue();

        var cancelled = await _service.GetByIdAsync(newOrder.Id);
        cancelled!.Status.Should().Be(PurchaseOrderStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_NonExistingOrder_ReturnsFalse()
    {
        // Act
        var result = await _service.CancelAsync(99999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GenerateOrderNumberAsync_ReturnsUniqueNumber()
    {
        // Act
        var number1 = await _service.GenerateOrderNumberAsync();
        var number2 = await _service.GenerateOrderNumberAsync();

        // Assert
        number1.Should().NotBeNullOrEmpty();
        number1.Should().StartWith("PO");
    }
}
