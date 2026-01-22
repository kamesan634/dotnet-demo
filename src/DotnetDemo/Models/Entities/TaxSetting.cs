namespace DotnetDemo.Models.Entities;

/// <summary>
/// 稅別設定
/// </summary>
public class TaxSetting
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 稅別名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 稅別代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 稅率 (例如: 0.05 代表 5%)
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// 是否為預設稅別
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 是否內含稅
    /// </summary>
    public bool IsInclusive { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

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
}
