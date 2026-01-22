using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 系統參數服務介面
/// </summary>
public interface ISystemSettingService
{
    /// <summary>
    /// 取得所有參數
    /// </summary>
    Task<List<SystemSetting>> GetAllAsync();

    /// <summary>
    /// 依群組取得參數
    /// </summary>
    Task<List<SystemSetting>> GetByGroupAsync(string group);

    /// <summary>
    /// 依鍵值取得參數
    /// </summary>
    Task<SystemSetting?> GetByKeyAsync(string key);

    /// <summary>
    /// 取得參數值
    /// </summary>
    Task<string?> GetValueAsync(string key);

    /// <summary>
    /// 取得整數參數值
    /// </summary>
    Task<int> GetIntValueAsync(string key, int defaultValue = 0);

    /// <summary>
    /// 取得布林參數值
    /// </summary>
    Task<bool> GetBoolValueAsync(string key, bool defaultValue = false);

    /// <summary>
    /// 設定參數值
    /// </summary>
    Task<bool> SetValueAsync(string key, string value, int? userId = null);

    /// <summary>
    /// 新增參數
    /// </summary>
    Task<SystemSetting> CreateAsync(SystemSetting setting);

    /// <summary>
    /// 更新參數
    /// </summary>
    Task<bool> UpdateAsync(SystemSetting setting, int? userId = null);

    /// <summary>
    /// 刪除參數
    /// </summary>
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
