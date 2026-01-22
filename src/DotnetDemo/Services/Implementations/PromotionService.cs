using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 促銷活動服務實作
/// </summary>
public class PromotionService : IPromotionService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public PromotionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<Promotion>> SearchAsync(string? keyword, bool? isActive, DateTime? date)
    {
        var query = _context.Promotions
            .Include(x => x.Store)
            .Include(x => x.CustomerLevel)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) || x.Code.Contains(keyword));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        if (date.HasValue)
        {
            query = query.Where(x => x.StartDate <= date.Value && x.EndDate >= date.Value);
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Promotion>> GetAllAsync()
    {
        return await _context.Promotions
            .Include(x => x.Store)
            .Include(x => x.CustomerLevel)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Promotion>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Promotions
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .Where(x => x.IsActive && x.StartDate <= now && x.EndDate >= now)
            .Where(x => x.UsageLimit == null || x.UsageCount < x.UsageLimit)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Promotion?> GetByIdAsync(int id)
    {
        return await _context.Promotions
            .Include(x => x.Store)
            .Include(x => x.CustomerLevel)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<List<Promotion>> GetByProductAsync(int productId, int? customerLevelId = null)
    {
        var now = DateTime.UtcNow;
        var query = _context.Promotions
            .Include(x => x.Items)
            .Where(x => x.IsActive && x.StartDate <= now && x.EndDate >= now)
            .Where(x => x.UsageLimit == null || x.UsageCount < x.UsageLimit)
            .Where(x => x.Items.Any(i => i.ProductId == productId) || !x.Items.Any());

        if (customerLevelId.HasValue)
        {
            query = query.Where(x => x.CustomerLevelId == null || x.CustomerLevelId == customerLevelId.Value);
        }
        else
        {
            query = query.Where(x => x.CustomerLevelId == null);
        }

        return await query.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Promotion> CreateAsync(Promotion promotion)
    {
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();
        return promotion;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Promotion promotion)
    {
        var existing = await _context.Promotions
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == promotion.Id);

        if (existing == null) return false;

        existing.Name = promotion.Name;
        existing.Code = promotion.Code;
        existing.Description = promotion.Description;
        existing.Type = promotion.Type;
        existing.DiscountValue = promotion.DiscountValue;
        existing.MinimumAmount = promotion.MinimumAmount;
        existing.MinimumQuantity = promotion.MinimumQuantity;
        existing.StartDate = promotion.StartDate;
        existing.EndDate = promotion.EndDate;
        existing.StoreId = promotion.StoreId;
        existing.CustomerLevelId = promotion.CustomerLevelId;
        existing.UsageLimit = promotion.UsageLimit;
        existing.IsActive = promotion.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.PromotionItems.RemoveRange(existing.Items);
        foreach (var item in promotion.Items)
        {
            item.PromotionId = existing.Id;
            _context.PromotionItems.Add(item);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var promotion = await _context.Promotions
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (promotion == null)
            return (false, "促銷活動不存在");

        _context.PromotionItems.RemoveRange(promotion.Items);
        _context.Promotions.Remove(promotion);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    /// <inheritdoc/>
    public async Task<decimal> CalculateDiscountAsync(int promotionId, decimal amount, int quantity)
    {
        var promotion = await GetByIdAsync(promotionId);
        if (promotion == null) return 0;

        if (promotion.MinimumAmount.HasValue && amount < promotion.MinimumAmount.Value)
            return 0;

        if (promotion.MinimumQuantity.HasValue && quantity < promotion.MinimumQuantity.Value)
            return 0;

        return promotion.Type switch
        {
            PromotionType.Discount => amount * (promotion.DiscountValue / 100),
            PromotionType.AmountOff => Math.Min(promotion.DiscountValue, amount),
            _ => 0
        };
    }
}
