namespace DotnetDemo.Models.Entities;

/// <summary>
/// 訂單狀態
/// </summary>
public enum OrderStatus
{
    /// <summary>待處理</summary>
    Pending = 1,
    /// <summary>已確認</summary>
    Confirmed = 2,
    /// <summary>已完成</summary>
    Completed = 3,
    /// <summary>已取消</summary>
    Cancelled = 4,
    /// <summary>已退貨</summary>
    Refunded = 5
}

/// <summary>
/// 訂單
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 訂單號碼
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// 門市編號
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// 門市
    /// </summary>
    public Store Store { get; set; } = null!;

    /// <summary>
    /// 客戶編號
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// 客戶
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// 商品小計
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// 折扣金額
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 使用點數
    /// </summary>
    public int PointsUsed { get; set; }

    /// <summary>
    /// 獲得點數
    /// </summary>
    public int PointsEarned { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 訂單明細
    /// </summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /// <summary>
    /// 付款記錄
    /// </summary>
    public ICollection<OrderPayment> Payments { get; set; } = new List<OrderPayment>();
}
