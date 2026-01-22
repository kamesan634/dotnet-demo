using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 促銷活動服務介面
/// </summary>
public interface IPromotionService
{
    /// <summary>
    /// 搜尋促銷活動
    /// </summary>
    Task<List<Promotion>> SearchAsync(string? keyword, bool? isActive, DateTime? date);

    /// <summary>
    /// 取得所有促銷活動
    /// </summary>
    Task<List<Promotion>> GetAllAsync();

    /// <summary>
    /// 取得目前有效的促銷活動
    /// </summary>
    Task<List<Promotion>> GetActiveAsync();

    /// <summary>
    /// 依編號取得促銷活動
    /// </summary>
    Task<Promotion?> GetByIdAsync(int id);

    /// <summary>
    /// 取得商品適用的促銷活動
    /// </summary>
    Task<List<Promotion>> GetByProductAsync(int productId, int? customerLevelId = null);

    /// <summary>
    /// 新增促銷活動
    /// </summary>
    Task<Promotion> CreateAsync(Promotion promotion);

    /// <summary>
    /// 更新促銷活動
    /// </summary>
    Task<bool> UpdateAsync(Promotion promotion);

    /// <summary>
    /// 刪除促銷活動
    /// </summary>
    Task<(bool Success, string? Error)> DeleteAsync(int id);

    /// <summary>
    /// 計算促銷折扣
    /// </summary>
    Task<decimal> CalculateDiscountAsync(int promotionId, decimal amount, int quantity);
}
