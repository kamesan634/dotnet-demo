using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Implementations;
using DotnetDemo.Services.Interfaces;
using DotnetDemo.Tests.TestHelpers;
using Xunit;

namespace DotnetDemo.Tests.Services;

/// <summary>
/// 報表服務測試 (100% 覆蓋率目標)
/// </summary>
public class ReportServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ReportService _service;
    private readonly Mock<ILogger<ReportService>> _loggerMock;

    public ReportServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<ReportService>>();
        _service = new ReportService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region GetDashboardDataAsync Tests

    [Fact]
    public async Task GetDashboardDataAsync_ReturnsDashboardData()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDashboardDataAsync_ReturnsTodaySales()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        result.TodaySales.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetDashboardDataAsync_ReturnsTodayOrders()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        result.TodayOrders.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetDashboardDataAsync_ReturnsTotalProducts()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        result.TotalProducts.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetDashboardDataAsync_ReturnsLowStockCount()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        result.LowStockCount.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetDashboardDataAsync_ReturnsLowStockItems()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        result.LowStockItems.Should().NotBeNull();
        if (result.LowStockItems.Any())
        {
            result.LowStockItems.Should().AllSatisfy(item =>
            {
                item.ProductName.Should().NotBeNullOrEmpty();
                item.Quantity.Should().BeLessThan(item.SafetyStock);
            });
        }
    }

    [Fact]
    public async Task GetDashboardDataAsync_ReturnsRecentOrders()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        result.RecentOrders.Should().NotBeNull();
        result.RecentOrders.Should().HaveCountLessOrEqualTo(10);
    }

    [Fact]
    public async Task GetDashboardDataAsync_RecentOrdersHaveValidData()
    {
        // Act
        var result = await _service.GetDashboardDataAsync();

        // Assert
        if (result.RecentOrders.Any())
        {
            result.RecentOrders.Should().AllSatisfy(order =>
            {
                order.OrderNumber.Should().NotBeNullOrEmpty();
                order.TotalAmount.Should().BeGreaterOrEqualTo(0);
                order.Status.Should().NotBeNullOrEmpty();
            });
        }
    }

    #endregion

    #region GetSalesDailyReportAsync Tests

    [Fact]
    public async Task GetSalesDailyReportAsync_ReturnsReport()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetSalesDailyReportAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSalesDailyReportAsync_WithSalesData_ReturnsGroupedData()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetSalesDailyReportAsync(startDate, endDate);

        // Assert
        if (result.Any())
        {
            result.Should().AllSatisfy(r =>
            {
                r.OrderCount.Should().BeGreaterThan(0);
                r.TotalAmount.Should().BeGreaterOrEqualTo(0);
            });
        }
    }

    [Fact]
    public async Task GetSalesDailyReportAsync_ReturnsOrderedByDate()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-7);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetSalesDailyReportAsync(startDate, endDate);

        // Assert
        result.Should().BeInAscendingOrder(r => r.Date);
    }

    [Fact]
    public async Task GetSalesDailyReportAsync_CalculatesNetAmount()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetSalesDailyReportAsync(startDate, endDate);

        // Assert
        result.Should().AllSatisfy(r =>
        {
            r.NetAmount.Should().Be(r.TotalAmount - r.DiscountAmount);
        });
    }

    #endregion

    #region GetProductRankingAsync Tests

    [Fact]
    public async Task GetProductRankingAsync_ReturnsReport()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetProductRankingAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProductRankingAsync_RespectsTopParameter()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetProductRankingAsync(startDate, endDate, 5);

        // Assert
        result.Should().HaveCountLessOrEqualTo(5);
    }

    [Fact]
    public async Task GetProductRankingAsync_OrderedByAmountDescending()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetProductRankingAsync(startDate, endDate);

        // Assert
        result.Should().BeInDescendingOrder(r => r.Amount);
    }

    [Fact]
    public async Task GetProductRankingAsync_AssignsCorrectRanks()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetProductRankingAsync(startDate, endDate);

        // Assert
        for (int i = 0; i < result.Count; i++)
        {
            result[i].Rank.Should().Be(i + 1);
        }
    }

    [Fact]
    public async Task GetProductRankingAsync_HasValidProductData()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetProductRankingAsync(startDate, endDate);

        // Assert
        result.Should().AllSatisfy(r =>
        {
            r.ProductCode.Should().NotBeNullOrEmpty();
            r.ProductName.Should().NotBeNullOrEmpty();
            r.Quantity.Should().BeGreaterOrEqualTo(0);
            r.Amount.Should().BeGreaterOrEqualTo(0);
        });
    }

    #endregion

    #region GetInventoryReportAsync Tests

    [Fact]
    public async Task GetInventoryReportAsync_ReturnsReport()
    {
        // Act
        var result = await _service.GetInventoryReportAsync(null, false);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetInventoryReportAsync_WithWarehouseFilter_ReturnsFilteredData()
    {
        // Act
        var result = await _service.GetInventoryReportAsync(1, false);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().OnlyContain(r => r.WarehouseName == "測試倉庫");
    }

    [Fact]
    public async Task GetInventoryReportAsync_LowStockOnly_ReturnsLowStockItems()
    {
        // Act
        var result = await _service.GetInventoryReportAsync(null, true);

        // Assert
        result.Should().OnlyContain(r => r.IsLowStock);
    }

    [Fact]
    public async Task GetInventoryReportAsync_CalculatesStockValue()
    {
        // Act
        var result = await _service.GetInventoryReportAsync(null, false);

        // Assert
        result.Should().AllSatisfy(r =>
        {
            r.StockValue.Should().BeGreaterOrEqualTo(0);
        });
    }

    [Fact]
    public async Task GetInventoryReportAsync_IdentifiesLowStock()
    {
        // Act
        var result = await _service.GetInventoryReportAsync(null, false);

        // Assert
        result.Should().AllSatisfy(r =>
        {
            r.IsLowStock.Should().Be(r.Quantity < r.SafetyStock);
        });
    }

    [Fact]
    public async Task GetInventoryReportAsync_OrderedByProductCode()
    {
        // Act
        var result = await _service.GetInventoryReportAsync(null, false);

        // Assert
        result.Should().BeInAscendingOrder(r => r.ProductCode);
    }

    #endregion

    #region GetPurchasingSummaryAsync Tests

    [Fact]
    public async Task GetPurchasingSummaryAsync_ReturnsSummary()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetPurchasingSummaryAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPurchasingSummaryAsync_ReturnsTotalOrders()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetPurchasingSummaryAsync(startDate, endDate);

        // Assert
        result.TotalOrders.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetPurchasingSummaryAsync_ReturnsTotalAmount()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetPurchasingSummaryAsync(startDate, endDate);

        // Assert
        result.TotalAmount.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetPurchasingSummaryAsync_ReturnsPendingOrders()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetPurchasingSummaryAsync(startDate, endDate);

        // Assert
        result.PendingOrders.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetPurchasingSummaryAsync_ReturnsCompletedOrders()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetPurchasingSummaryAsync(startDate, endDate);

        // Assert
        result.CompletedOrders.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetPurchasingSummaryAsync_ReturnsTopSuppliers()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetPurchasingSummaryAsync(startDate, endDate);

        // Assert
        result.TopSuppliers.Should().NotBeNull();
        result.TopSuppliers.Should().HaveCountLessOrEqualTo(5);
    }

    [Fact]
    public async Task GetPurchasingSummaryAsync_TopSuppliersOrderedByAmount()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetPurchasingSummaryAsync(startDate, endDate);

        // Assert
        result.TopSuppliers.Should().BeInDescendingOrder(s => s.TotalAmount);
    }

    #endregion

    #region GetCustomerSpendingReportAsync Tests

    [Fact]
    public async Task GetCustomerSpendingReportAsync_ReturnsReport()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetCustomerSpendingReportAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCustomerSpendingReportAsync_RespectsTopParameter()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetCustomerSpendingReportAsync(startDate, endDate, 5);

        // Assert
        result.Should().HaveCountLessOrEqualTo(5);
    }

    [Fact]
    public async Task GetCustomerSpendingReportAsync_OrderedBySpentDescending()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetCustomerSpendingReportAsync(startDate, endDate);

        // Assert
        result.Should().BeInDescendingOrder(r => r.TotalSpent);
    }

    [Fact]
    public async Task GetCustomerSpendingReportAsync_AssignsCorrectRanks()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetCustomerSpendingReportAsync(startDate, endDate);

        // Assert
        for (int i = 0; i < result.Count; i++)
        {
            result[i].Rank.Should().Be(i + 1);
        }
    }

    [Fact]
    public async Task GetCustomerSpendingReportAsync_HasValidCustomerData()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-30);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetCustomerSpendingReportAsync(startDate, endDate);

        // Assert
        result.Should().AllSatisfy(r =>
        {
            r.CustomerCode.Should().NotBeNullOrEmpty();
            r.CustomerName.Should().NotBeNullOrEmpty();
            r.OrderCount.Should().BeGreaterOrEqualTo(0);
            r.TotalSpent.Should().BeGreaterOrEqualTo(0);
        });
    }

    #endregion
}
