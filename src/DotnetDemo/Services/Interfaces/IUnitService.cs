using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 單位服務介面
/// </summary>
public interface IUnitService
{
    /// <summary>
    /// 取得所有單位
    /// </summary>
    Task<List<Unit>> GetAllAsync();

    /// <summary>
    /// 取得啟用中的單位
    /// </summary>
    Task<List<Unit>> GetActiveAsync();

    /// <summary>
    /// 依編號取得單位
    /// </summary>
    Task<Unit?> GetByIdAsync(int id);

    /// <summary>
    /// 新增單位
    /// </summary>
    Task<Unit> CreateAsync(Unit unit);

    /// <summary>
    /// 更新單位
    /// </summary>
    Task<bool> UpdateAsync(Unit unit);

    /// <summary>
    /// 刪除單位
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 檢查代碼是否存在
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}
