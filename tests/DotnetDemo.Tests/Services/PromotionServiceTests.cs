using FluentAssertions;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Implementations;
using DotnetDemo.Tests.TestHelpers;
using Xunit;

namespace DotnetDemo.Tests.Services;

/// <summary>
/// 促銷活動服務測試
/// </summary>
public class PromotionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PromotionService _service;

    public PromotionServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _service = new PromotionService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingPromotions()
    {
        // Arrange
        var promotion = new Promotion
        {
            Name = "週年慶優惠",
            Code = "ANNIV2024",
            Type = PromotionType.Discount,
            DiscountValue = 10,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(30),
            IsActive = true
        };
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SearchAsync("週年慶", null, null);

        // Assert
        result.Should().ContainSingle();
        result[0].Name.Should().Be("週年慶優惠");
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActivePromotions()
    {
        // Arrange
        var activePromo = new Promotion
        {
            Name = "進行中活動",
            Code = "ACTIVE01",
            Type = PromotionType.Discount,
            DiscountValue = 5,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(30),
            IsActive = true
        };
        var inactivePromo = new Promotion
        {
            Name = "已停用活動",
            Code = "INACTIVE01",
            Type = PromotionType.Discount,
            DiscountValue = 5,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(30),
            IsActive = false
        };
        _context.Promotions.AddRange(activePromo, inactivePromo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetActiveAsync();

        // Assert
        result.Should().Contain(p => p.Code == "ACTIVE01");
        result.Should().NotContain(p => p.Code == "INACTIVE01");
    }

    [Fact]
    public async Task CreateAsync_CreatesPromotionSuccessfully()
    {
        // Arrange
        var promotion = new Promotion
        {
            Name = "新品上市",
            Code = "NEW2024",
            Type = PromotionType.AmountOff,
            DiscountValue = 100,
            MinimumAmount = 500,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(14),
            IsActive = true
        };

        // Act
        var result = await _service.CreateAsync(promotion);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Code.Should().Be("NEW2024");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPromotionSuccessfully()
    {
        // Arrange
        var promotion = new Promotion
        {
            Name = "測試活動",
            Code = "TEST01",
            Type = PromotionType.Discount,
            DiscountValue = 10,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7),
            IsActive = true
        };
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        // Act
        promotion.Name = "更新後活動";
        promotion.DiscountValue = 15;
        var result = await _service.UpdateAsync(promotion);

        // Assert
        result.Should().BeTrue();
        var updated = await _service.GetByIdAsync(promotion.Id);
        updated!.Name.Should().Be("更新後活動");
        updated.DiscountValue.Should().Be(15);
    }

    [Fact]
    public async Task DeleteAsync_DeletesPromotionSuccessfully()
    {
        // Arrange
        var promotion = new Promotion
        {
            Name = "待刪除活動",
            Code = "DELETE01",
            Type = PromotionType.Discount,
            DiscountValue = 5,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7),
            IsActive = true
        };
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        // Act
        var (success, error) = await _service.DeleteAsync(promotion.Id);

        // Assert
        success.Should().BeTrue();
        error.Should().BeNull();
        var deleted = await _service.GetByIdAsync(promotion.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task CalculateDiscountAsync_CalculatesDiscountCorrectly()
    {
        // Arrange
        var promotion = new Promotion
        {
            Name = "折扣活動",
            Code = "DISC10",
            Type = PromotionType.Discount,
            DiscountValue = 10,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(30),
            IsActive = true
        };
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        // Act
        var discount = await _service.CalculateDiscountAsync(promotion.Id, 1000, 1);

        // Assert
        discount.Should().Be(100); // 1000 * 10%
    }

    [Fact]
    public async Task CalculateDiscountAsync_WithMinimumAmount_ReturnsZeroIfNotMet()
    {
        // Arrange
        var promotion = new Promotion
        {
            Name = "滿額折活動",
            Code = "MIN500",
            Type = PromotionType.AmountOff,
            DiscountValue = 50,
            MinimumAmount = 500,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(30),
            IsActive = true
        };
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        // Act
        var discount = await _service.CalculateDiscountAsync(promotion.Id, 300, 1);

        // Assert
        discount.Should().Be(0); // 未達最低消費
    }
}
