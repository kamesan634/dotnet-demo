namespace DotnetDemo.Models.Entities;

/// <summary>
/// 客戶/會員
/// </summary>
public class Customer
{
    /// <summary>
    /// 客戶編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 會員卡號
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateOnly? Birthday { get; set; }

    /// <summary>
    /// 會員等級編號
    /// </summary>
    public int CustomerLevelId { get; set; }

    /// <summary>
    /// 會員等級
    /// </summary>
    public CustomerLevel CustomerLevel { get; set; } = null!;

    /// <summary>
    /// 累積點數
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 累積消費金額
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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
    /// 訂單
    /// </summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
