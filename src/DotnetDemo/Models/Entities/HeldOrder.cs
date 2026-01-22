namespace DotnetDemo.Models.Entities;

/// <summary>
/// 掛單
/// </summary>
public class HeldOrder
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 掛單號碼
    /// </summary>
    public string HoldNumber { get; set; } = string.Empty;

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
    /// 掛單資料 (JSON 格式儲存商品明細)
    /// </summary>
    public string OrderData { get; set; } = string.Empty;

    /// <summary>
    /// 暫存金額
    /// </summary>
    public decimal TotalAmount { get; set; }

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
    /// 過期時間
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
