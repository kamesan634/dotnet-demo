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
/// 訂單服務測試
/// </summary>
public class OrderServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly OrderService _service;
    private readonly Mock<ILogger<OrderService>> _loggerMock;

    public OrderServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _service = new OrderService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchAsync_NoFilter_ReturnsOrders()
    {
        // Act
        var result = await _service.SearchAsync(null, null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task SearchAsync_WithOrderNumber_ReturnsMatchingOrders()
    {
        // Act
        var result = await _service.SearchAsync("ORD202401010001", null, null, null);

        // Assert
        result.Should().ContainSingle();
        result[0].OrderNumber.Should().Be("ORD202401010001");
    }

    [Fact]
    public async Task SearchAsync_WithStatus_ReturnsFilteredOrders()
    {
        // Act
        var result = await _service.SearchAsync(null, OrderStatus.Completed, null, null);

        // Assert
        result.Should().OnlyContain(o => o.Status == OrderStatus.Completed);
    }

    [Fact]
    public async Task SearchAsync_WithDateRange_ReturnsFilteredOrders()
    {
        // Act
        var result = await _service.SearchAsync(null, null, DateTime.UtcNow.Date, DateTime.UtcNow.Date);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsOrder()
    {
        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Items.Should().NotBeEmpty();
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
    public async Task GetByOrderNumberAsync_ExistingNumber_ReturnsOrder()
    {
        // Act
        var result = await _service.GetByOrderNumberAsync("ORD202401010001");

        // Assert
        result.Should().NotBeNull();
        result!.OrderNumber.Should().Be("ORD202401010001");
    }

    [Fact]
    public async Task GetByOrderNumberAsync_NonExistingNumber_ReturnsNull()
    {
        // Act
        var result = await _service.GetByOrderNumberAsync("NOTEXIST");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_ReturnsCreatedOrder()
    {
        // Arrange
        var order = new Order
        {
            StoreId = 1,
            CustomerId = 1,
            Status = OrderStatus.Pending,
            SubTotal = 100,
            TotalAmount = 100,
            CreatedBy = 1
        };

        // Act
        var result = await _service.CreateAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.OrderNumber.Should().NotBeNullOrEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateStatusAsync_ExistingOrder_ReturnsTrue()
    {
        // Act
        var result = await _service.UpdateStatusAsync(2, OrderStatus.Completed);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByIdAsync(2);
        updated!.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public async Task UpdateStatusAsync_NonExistingOrder_ReturnsFalse()
    {
        // Act
        var result = await _service.UpdateStatusAsync(99999, OrderStatus.Completed);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTodayStatsAsync_ReturnsStats()
    {
        // Act
        var result = await _service.GetTodayStatsAsync();

        // Assert
        result.Count.Should().BeGreaterOrEqualTo(0);
        result.Total.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetRecentAsync_ReturnsRecentOrders()
    {
        // Act
        var result = await _service.GetRecentAsync(5);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountLessOrEqualTo(5);
    }
}
