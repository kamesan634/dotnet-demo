namespace DotnetDemo.Models.Entities;

/// <summary>
/// 付款方式
/// </summary>
public class PaymentMethod
{
    /// <summary>
    /// 付款方式編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 付款方式代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 付款方式名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否需要找零
    /// </summary>
    public bool RequiresChange { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 訂單付款記錄
    /// </summary>
    public ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
}
