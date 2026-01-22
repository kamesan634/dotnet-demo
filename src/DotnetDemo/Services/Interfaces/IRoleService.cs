using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 角色服務介面
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 取得所有角色
    /// </summary>
    Task<List<ApplicationRole>> GetAllAsync();

    /// <summary>
    /// 依編號取得角色
    /// </summary>
    Task<ApplicationRole?> GetByIdAsync(int id);

    /// <summary>
    /// 依名稱取得角色
    /// </summary>
    Task<ApplicationRole?> GetByNameAsync(string name);

    /// <summary>
    /// 建立角色
    /// </summary>
    Task<(bool Success, string? Error)> CreateAsync(ApplicationRole role);

    /// <summary>
    /// 更新角色
    /// </summary>
    Task<(bool Success, string? Error)> UpdateAsync(ApplicationRole role);

    /// <summary>
    /// 刪除角色
    /// </summary>
    Task<(bool Success, string? Error)> DeleteAsync(int id);

    /// <summary>
    /// 取得角色的使用者數量
    /// </summary>
    Task<int> GetUserCountAsync(int roleId);
}
