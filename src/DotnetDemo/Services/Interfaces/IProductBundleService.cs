using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 商品組合服務介面
/// </summary>
public interface IProductBundleService
{
    /// <summary>
    /// 搜尋商品組合
    /// </summary>
    Task<List<ProductBundle>> SearchAsync(string? keyword, bool? isActive);

    /// <summary>
    /// 取得所有商品組合
    /// </summary>
    Task<List<ProductBundle>> GetAllAsync();

    /// <summary>
    /// 取得有效的商品組合
    /// </summary>
    Task<List<ProductBundle>> GetActiveAsync();

    /// <summary>
    /// 依編號取得商品組合
    /// </summary>
    Task<ProductBundle?> GetByIdAsync(int id);

    /// <summary>
    /// 新增商品組合
    /// </summary>
    Task<ProductBundle> CreateAsync(ProductBundle bundle);

    /// <summary>
    /// 更新商品組合
    /// </summary>
    Task<bool> UpdateAsync(ProductBundle bundle);

    /// <summary>
    /// 刪除商品組合
    /// </summary>
    Task<(bool Success, string? Error)> DeleteAsync(int id);

    /// <summary>
    /// 計算組合原價
    /// </summary>
    Task<decimal> CalculateOriginalTotalAsync(List<ProductBundleItem> items);
}
