namespace DotnetDemo.Models.Entities;

/// <summary>
/// 操作紀錄
/// </summary>
public class AuditLog
{
    /// <summary>
    /// 紀錄編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作類型
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 資源類型
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// 資源編號
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// 舊值 (JSON)
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// 新值 (JSON)
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// IP 位址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 使用者代理
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
