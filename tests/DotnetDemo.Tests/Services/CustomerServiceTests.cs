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
/// 客戶服務測試
/// </summary>
public class CustomerServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CustomerService _service;
    private readonly Mock<ILogger<CustomerService>> _loggerMock;

    public CustomerServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<CustomerService>>();
        _service = new CustomerService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCustomer()
    {
        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.CustomerLevel.Should().NotBeNull();
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
    public async Task CreateAsync_ValidCustomer_ReturnsCreatedCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Code = "M999",
            Name = "新客戶",
            Phone = "0999999999",
            CustomerLevelId = 1,
            IsActive = true
        };

        // Act
        var result = await _service.CreateAsync(customer);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_ExistingCustomer_ReturnsTrue()
    {
        // Arrange
        var customer = await _service.GetByIdAsync(1);
        customer!.Name = "更新後的名稱";

        // Act
        var result = await _service.UpdateAsync(customer);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByIdAsync(1);
        updated!.Name.Should().Be("更新後的名稱");
    }

    [Fact]
    public async Task UpdateAsync_NonExistingCustomer_ReturnsFalse()
    {
        // Arrange
        var customer = new Customer { Id = 99999, Code = "XXX", Name = "不存在", CustomerLevelId = 1 };

        // Act
        var result = await _service.UpdateAsync(customer);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingCustomer_ReturnsTrue()
    {
        // Arrange
        var newCustomer = await _service.CreateAsync(new Customer
        {
            Code = "DEL01",
            Name = "待刪除客戶",
            CustomerLevelId = 1,
            IsActive = true
        });

        // Act
        var result = await _service.DeleteAsync(newCustomer.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingCustomer_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteAsync(99999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdatePointsAsync_ExistingCustomer_UpdatesPoints()
    {
        // Arrange
        var originalPoints = (await _service.GetByIdAsync(1))!.Points;

        // Act
        var result = await _service.UpdatePointsAsync(1, 50);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByIdAsync(1);
        updated!.Points.Should().Be(originalPoints + 50);
    }

    [Fact]
    public async Task UpdatePointsAsync_NonExistingCustomer_ReturnsFalse()
    {
        // Act
        var result = await _service.UpdatePointsAsync(99999, 50);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateTotalSpentAsync_ExistingCustomer_UpdatesTotalSpent()
    {
        // Arrange
        var originalSpent = (await _service.GetByIdAsync(1))!.TotalSpent;

        // Act
        var result = await _service.UpdateTotalSpentAsync(1, 100m);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByIdAsync(1);
        updated!.TotalSpent.Should().Be(originalSpent + 100m);
    }

    [Fact]
    public async Task UpdateTotalSpentAsync_NonExistingCustomer_ReturnsFalse()
    {
        // Act
        var result = await _service.UpdateTotalSpentAsync(99999, 100m);

        // Assert
        result.Should().BeFalse();
    }
}
