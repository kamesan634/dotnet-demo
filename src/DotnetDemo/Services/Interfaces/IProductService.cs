using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 商品服務介面
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 取得所有商品
    /// </summary>
    Task<List<Product>> GetAllAsync();

    /// <summary>
    /// 依條件搜尋商品
    /// </summary>
    Task<List<Product>> SearchAsync(string? searchText, int? categoryId, bool? isActive);

    /// <summary>
    /// 依編號取得商品
    /// </summary>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// 新增商品
    /// </summary>
    Task<Product> CreateAsync(Product product);

    /// <summary>
    /// 更新商品
    /// </summary>
    Task<bool> UpdateAsync(Product product);

    /// <summary>
    /// 刪除商品
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 檢查代碼是否存在
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);

    /// <summary>
    /// POS 商品搜尋 (依名稱或條碼)
    /// </summary>
    /// <param name="searchText">搜尋文字</param>
    /// <param name="limit">回傳數量上限</param>
    Task<List<Product>> SearchForPosAsync(string searchText, int limit = 10);
}
