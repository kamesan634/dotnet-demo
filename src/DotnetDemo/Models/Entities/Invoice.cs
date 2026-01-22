namespace DotnetDemo.Models.Entities;

/// <summary>
/// 發票類型
/// </summary>
public enum InvoiceType
{
    /// <summary>二聯式發票</summary>
    Duplicate = 1,
    /// <summary>三聯式發票</summary>
    Triplicate = 2,
    /// <summary>電子發票</summary>
    Electronic = 3
}

/// <summary>
/// 發票狀態
/// </summary>
public enum InvoiceStatus
{
    /// <summary>已開立</summary>
    Issued = 1,
    /// <summary>已作廢</summary>
    Voided = 2,
    /// <summary>已折讓</summary>
    Allowance = 3
}

/// <summary>
/// 發票
/// </summary>
public class Invoice
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// 訂單編號
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 訂單
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// 發票類型
    /// </summary>
    public InvoiceType Type { get; set; }

    /// <summary>
    /// 發票狀態
    /// </summary>
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;

    /// <summary>
    /// 買受人統一編號
    /// </summary>
    public string? BuyerTaxId { get; set; }

    /// <summary>
    /// 買受人名稱
    /// </summary>
    public string? BuyerName { get; set; }

    /// <summary>
    /// 銷售額 (未稅)
    /// </summary>
    public decimal SalesAmount { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 開立日期
    /// </summary>
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 作廢日期
    /// </summary>
    public DateTime? VoidedAt { get; set; }

    /// <summary>
    /// 作廢原因
    /// </summary>
    public string? VoidReason { get; set; }

    /// <summary>
    /// 隨機碼
    /// </summary>
    public string? RandomNumber { get; set; }

    /// <summary>
    /// 載具類型
    /// </summary>
    public string? CarrierType { get; set; }

    /// <summary>
    /// 載具號碼
    /// </summary>
    public string? CarrierNumber { get; set; }

    /// <summary>
    /// 捐贈碼
    /// </summary>
    public string? DonationCode { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
