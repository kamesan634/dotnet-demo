using Microsoft.AspNetCore.Identity;

namespace DotnetDemo.Models.Entities;

/// <summary>
/// 應用程式使用者
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    /// <summary>
    /// 顯示名稱
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 所屬門市編號
    /// </summary>
    public int? StoreId { get; set; }

    /// <summary>
    /// 所屬門市
    /// </summary>
    public Store? Store { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
