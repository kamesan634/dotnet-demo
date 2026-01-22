using FluentAssertions;
using DotnetDemo.Data;
using DotnetDemo.Services.Implementations;
using DotnetDemo.Tests.TestHelpers;
using Xunit;

namespace DotnetDemo.Tests.Services;

/// <summary>
/// 匯出服務測試
/// </summary>
public class ExportServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ExportService _service;

    public ExportServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _service = new ExportService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task ExportToExcelAsync_ReturnsNonEmptyBytes()
    {
        // Arrange
        var data = new[]
        {
            new { Name = "商品1", Price = 100 },
            new { Name = "商品2", Price = 200 }
        };

        // Act
        var result = await _service.ExportToExcelAsync(data, "測試表");

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportToExcelAsync_WithColumnMappings_AppliesMappings()
    {
        // Arrange
        var data = new[]
        {
            new { Name = "商品1", Price = 100 }
        };
        var mappings = new Dictionary<string, string>
        {
            { "Name", "商品名稱" },
            { "Price", "價格" }
        };

        // Act
        var result = await _service.ExportToExcelAsync(data, "測試表", mappings);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        var content = System.Text.Encoding.UTF8.GetString(result.Skip(3).ToArray()); // Skip BOM
        content.Should().Contain("商品名稱");
        content.Should().Contain("價格");
    }

    [Fact]
    public async Task ExportToCsvAsync_ReturnsValidCsv()
    {
        // Arrange
        var data = new[]
        {
            new { Name = "商品1", Price = 100 },
            new { Name = "商品2", Price = 200 }
        };

        // Act
        var result = await _service.ExportToCsvAsync(data, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        var content = System.Text.Encoding.UTF8.GetString(result.Skip(3).ToArray()); // Skip BOM
        content.Should().Contain("Name,Price");
        content.Should().Contain("商品1,100");
        content.Should().Contain("商品2,200");
    }

    [Fact]
    public async Task ExportToCsvAsync_EscapesSpecialCharacters()
    {
        // Arrange
        var data = new[]
        {
            new { Name = "商品,有逗號", Price = 100 },
            new { Name = "商品\"有引號\"", Price = 200 }
        };

        // Act
        var result = await _service.ExportToCsvAsync(data, null);

        // Assert
        result.Should().NotBeNull();
        var content = System.Text.Encoding.UTF8.GetString(result.Skip(3).ToArray());
        content.Should().Contain("\"商品,有逗號\""); // 逗號應被引號包圍
        content.Should().Contain("\"商品\"\"有引號\"\"\""); // 引號應被跳脫
    }

    [Fact]
    public async Task ExportSalesReportAsync_ReturnsValidReport()
    {
        // Act
        var result = await _service.ExportSalesReportAsync(
            DateTime.Today.AddDays(-30),
            DateTime.Today.AddDays(1));

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportInventoryReportAsync_ReturnsValidReport()
    {
        // Act
        var result = await _service.ExportInventoryReportAsync(null);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportInventoryReportAsync_WithWarehouseFilter_ReturnsFilteredReport()
    {
        // Act
        var result = await _service.ExportInventoryReportAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportPurchasingReportAsync_ReturnsValidReport()
    {
        // Act
        var result = await _service.ExportPurchasingReportAsync(
            DateTime.Today.AddDays(-30),
            DateTime.Today.AddDays(1));

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
}
