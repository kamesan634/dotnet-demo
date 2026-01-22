using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 使用者服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 取得所有使用者
    /// </summary>
    Task<List<ApplicationUser>> GetAllAsync();

    /// <summary>
    /// 依條件搜尋使用者
    /// </summary>
    Task<List<ApplicationUser>> SearchAsync(string? searchText, int? storeId, bool? isActive);

    /// <summary>
    /// 依編號取得使用者
    /// </summary>
    Task<ApplicationUser?> GetByIdAsync(int id);

    /// <summary>
    /// 建立使用者
    /// </summary>
    Task<(bool Success, string? Error)> CreateAsync(ApplicationUser user, string password, IEnumerable<string> roles);

    /// <summary>
    /// 更新使用者
    /// </summary>
    Task<(bool Success, string? Error)> UpdateAsync(ApplicationUser user, IEnumerable<string>? roles = null);

    /// <summary>
    /// 刪除使用者
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 變更密碼
    /// </summary>
    Task<(bool Success, string? Error)> ChangePasswordAsync(int id, string newPassword);

    /// <summary>
    /// 取得使用者角色
    /// </summary>
    Task<IList<string>> GetRolesAsync(int userId);

    /// <summary>
    /// 更新最後登入時間
    /// </summary>
    Task UpdateLastLoginAsync(int userId);
}
