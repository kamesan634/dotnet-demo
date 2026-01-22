using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 門市服務介面
/// </summary>
public interface IStoreService
{
    /// <summary>
    /// 取得所有門市
    /// </summary>
    Task<List<Store>> GetAllAsync();

    /// <summary>
    /// 取得啟用中的門市
    /// </summary>
    Task<List<Store>> GetActiveAsync();

    /// <summary>
    /// 依編號取得門市
    /// </summary>
    Task<Store?> GetByIdAsync(int id);

    /// <summary>
    /// 新增門市
    /// </summary>
    Task<Store> CreateAsync(Store store);

    /// <summary>
    /// 更新門市
    /// </summary>
    Task<bool> UpdateAsync(Store store);

    /// <summary>
    /// 刪除門市
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 檢查代碼是否存在
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}
