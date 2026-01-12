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
/// 供應商服務測試
/// </summary>
public class SupplierServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SupplierService _service;
    private readonly Mock<ILogger<SupplierService>> _loggerMock;

    public SupplierServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<SupplierService>>();
        _service = new SupplierService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSuppliers()
    {
        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActiveSuppliers()
    {
        // Act
        var result = await _service.GetActiveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(s => s.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsSupplier()
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
    public async Task CreateAsync_ValidSupplier_ReturnsCreatedSupplier()
    {
        // Arrange
        var supplier = new Supplier
        {
            Code = "SUP999",
            Name = "新供應商",
            ContactName = "王五",
            Phone = "02-9999999",
            IsActive = true
        };

        // Act
        var result = await _service.CreateAsync(supplier);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_ExistingSupplier_ReturnsTrue()
    {
        // Arrange
        var supplier = await _service.GetByIdAsync(1);
        supplier!.Name = "更新後的供應商";

        // Act
        var result = await _service.UpdateAsync(supplier);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByIdAsync(1);
        updated!.Name.Should().Be("更新後的供應商");
    }

    [Fact]
    public async Task UpdateAsync_NonExistingSupplier_ReturnsFalse()
    {
        // Arrange
        var supplier = new Supplier { Id = 99999, Code = "XXX", Name = "不存在" };

        // Act
        var result = await _service.UpdateAsync(supplier);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingSupplier_ReturnsTrue()
    {
        // Arrange
        var newSupplier = await _service.CreateAsync(new Supplier
        {
            Code = "DEL01",
            Name = "待刪除供應商",
            IsActive = true
        });

        // Act
        var result = await _service.DeleteAsync(newSupplier.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingSupplier_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteAsync(99999);

        // Assert
        result.Should().BeFalse();
    }
}
