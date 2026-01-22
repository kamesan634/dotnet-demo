using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 稅別設定服務介面
/// </summary>
public interface ITaxSettingService
{
    /// <summary>
    /// 取得所有稅別
    /// </summary>
    Task<List<TaxSetting>> GetAllAsync();

    /// <summary>
    /// 取得啟用的稅別
    /// </summary>
    Task<List<TaxSetting>> GetActiveAsync();

    /// <summary>
    /// 依編號取得稅別
    /// </summary>
    Task<TaxSetting?> GetByIdAsync(int id);

    /// <summary>
    /// 取得預設稅別
    /// </summary>
    Task<TaxSetting?> GetDefaultAsync();

    /// <summary>
    /// 新增稅別
    /// </summary>
    Task<TaxSetting> CreateAsync(TaxSetting taxSetting);

    /// <summary>
    /// 更新稅別
    /// </summary>
    Task<bool> UpdateAsync(TaxSetting taxSetting);

    /// <summary>
    /// 刪除稅別
    /// </summary>
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
