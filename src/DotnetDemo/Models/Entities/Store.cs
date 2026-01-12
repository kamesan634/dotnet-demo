namespace DotnetDemo.Models.Entities;

/// <summary>
/// 門市
/// </summary>
public class Store
{
    /// <summary>
    /// 門市編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 門市代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 門市名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 門市地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 是否為主要門市
    /// </summary>
    public bool IsPrimary { get; set; } = false;

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
    /// 門市使用者
    /// </summary>
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
