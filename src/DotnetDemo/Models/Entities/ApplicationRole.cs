using Microsoft.AspNetCore.Identity;

namespace DotnetDemo.Models.Entities;

/// <summary>
/// 應用程式角色
/// </summary>
public class ApplicationRole : IdentityRole<int>
{
    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否為系統角色
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
