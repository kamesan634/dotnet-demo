namespace DotnetDemo.Models.Entities;

/// <summary>
/// 供應商
/// </summary>
public class Supplier
{
    /// <summary>
    /// 供應商編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 供應商代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactName { get; set; }

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
    /// 統一編號
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// 付款條件 (天數)
    /// </summary>
    public int PaymentTermDays { get; set; } = 30;

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
    /// 供應的商品價格
    /// </summary>
    public ICollection<SupplierPrice> SupplierPrices { get; set; } = new List<SupplierPrice>();

    /// <summary>
    /// 採購單
    /// </summary>
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
