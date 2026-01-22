using FluentAssertions;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Implementations;
using DotnetDemo.Tests.TestHelpers;
using Xunit;

namespace DotnetDemo.Tests.Services;

/// <summary>
/// 採購退貨服務測試
/// </summary>
public class PurchaseReturnServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PurchaseReturnService _service;

    public PurchaseReturnServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _service = new PurchaseReturnService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllReturns()
    {
        // Arrange
        var purchaseReturn = new PurchaseReturn
        {
            ReturnNumber = "PR001",
            SupplierId = 1,
            WarehouseId = 1,
            Reason = "品質問題",
            Status = PurchaseReturnStatus.Pending,
            CreatedBy = 1
        };
        _context.PurchaseReturns.Add(purchaseReturn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchAsync(null, null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task CreateAsync_CreatesPurchaseReturnWithItems()
    {
        // Arrange
        var purchaseReturn = new PurchaseReturn
        {
            SupplierId = 1,
            WarehouseId = 1,
            Reason = "瑕疵品",
            Items = new List<PurchaseReturnItem>
            {
                new PurchaseReturnItem
                {
                    ProductId = 1,
                    Quantity = 5,
                    UnitPrice = 100,
                    SubTotal = 500
                }
            }
        };

        // Act
        var result = await _service.CreateAsync(purchaseReturn, 1);

        // Assert
        result.Should().NotBeNull();
        result.ReturnNumber.Should().NotBeNullOrEmpty();
        result.Status.Should().Be(PurchaseReturnStatus.Pending);
        result.TotalAmount.Should().Be(500);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsReturnWithItems()
    {
        // Arrange
        var purchaseReturn = new PurchaseReturn
        {
            ReturnNumber = "PR002",
            SupplierId = 1,
            WarehouseId = 1,
            Reason = "數量錯誤",
            Status = PurchaseReturnStatus.Pending,
            CreatedBy = 1,
            Items = new List<PurchaseReturnItem>
            {
                new PurchaseReturnItem
                {
                    ProductId = 1,
                    Quantity = 10,
                    UnitPrice = 50,
                    SubTotal = 500
                }
            }
        };
        _context.PurchaseReturns.Add(purchaseReturn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(purchaseReturn.Id);

        // Assert
        result.Should().NotBeNull();
        result!.ReturnNumber.Should().Be("PR002");
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CompleteAsync_CompletesConfirmedReturn()
    {
        // Arrange
        var purchaseReturn = new PurchaseReturn
        {
            ReturnNumber = "PR003",
            SupplierId = 1,
            WarehouseId = 1,
            Reason = "退貨",
            Status = PurchaseReturnStatus.Confirmed,
            CreatedBy = 1
        };
        _context.PurchaseReturns.Add(purchaseReturn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CompleteAsync(purchaseReturn.Id, 1);

        // Assert
        result.Should().BeTrue();
        var completed = await _service.GetByIdAsync(purchaseReturn.Id);
        completed!.Status.Should().Be(PurchaseReturnStatus.Returned);
    }

    [Fact]
    public async Task CancelAsync_CancelsPendingReturn()
    {
        // Arrange
        var purchaseReturn = new PurchaseReturn
        {
            ReturnNumber = "PR004",
            SupplierId = 1,
            WarehouseId = 1,
            Reason = "取消測試",
            Status = PurchaseReturnStatus.Pending,
            CreatedBy = 1
        };
        _context.PurchaseReturns.Add(purchaseReturn);
        await _context.SaveChangesAsync();

        // Act
        var (success, error) = await _service.CancelAsync(purchaseReturn.Id, 1);

        // Assert
        success.Should().BeTrue();
        error.Should().BeNull();
        var cancelled = await _service.GetByIdAsync(purchaseReturn.Id);
        cancelled!.Status.Should().Be(PurchaseReturnStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_CannotCancelCompletedReturn()
    {
        // Arrange
        var purchaseReturn = new PurchaseReturn
        {
            ReturnNumber = "PR005",
            SupplierId = 1,
            WarehouseId = 1,
            Reason = "已完成退貨",
            Status = PurchaseReturnStatus.Returned,
            CreatedBy = 1
        };
        _context.PurchaseReturns.Add(purchaseReturn);
        await _context.SaveChangesAsync();

        // Act
        var (success, error) = await _service.CancelAsync(purchaseReturn.Id, 1);

        // Assert
        success.Should().BeFalse();
        error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateReturnNumberAsync_ReturnsUniqueNumber()
    {
        // Act
        var number = await _service.GenerateReturnNumberAsync();

        // Assert
        number.Should().NotBeNullOrEmpty();
        number.Should().StartWith("PR");
    }
}
