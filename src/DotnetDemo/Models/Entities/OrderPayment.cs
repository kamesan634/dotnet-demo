namespace DotnetDemo.Models.Entities;

/// <summary>
/// 訂單付款記錄
/// </summary>
public class OrderPayment
{
    /// <summary>
    /// 付款記錄編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 訂單
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// 付款方式編號
    /// </summary>
    public int PaymentMethodId { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; } = null!;

    /// <summary>
    /// 付款金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
