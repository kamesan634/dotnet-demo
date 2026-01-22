using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 發票服務介面
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// 搜尋發票
    /// </summary>
    Task<List<Invoice>> SearchAsync(string? keyword, InvoiceType? type, InvoiceStatus? status, DateTime? from, DateTime? to);

    /// <summary>
    /// 依編號取得發票
    /// </summary>
    Task<Invoice?> GetByIdAsync(int id);

    /// <summary>
    /// 依訂單取得發票
    /// </summary>
    Task<Invoice?> GetByOrderIdAsync(int orderId);

    /// <summary>
    /// 開立發票
    /// </summary>
    Task<Invoice> CreateAsync(Invoice invoice);

    /// <summary>
    /// 作廢發票
    /// </summary>
    Task<(bool Success, string? Error)> VoidAsync(int id, string reason, int userId);

    /// <summary>
    /// 產生發票號碼
    /// </summary>
    Task<string> GenerateInvoiceNumberAsync();

    /// <summary>
    /// 取得發票統計
    /// </summary>
    Task<InvoiceStats> GetStatsAsync(DateTime from, DateTime to);
}

/// <summary>
/// 發票統計
/// </summary>
public class InvoiceStats
{
    /// <summary>開立數量</summary>
    public int IssuedCount { get; set; }
    /// <summary>作廢數量</summary>
    public int VoidedCount { get; set; }
    /// <summary>開立金額</summary>
    public decimal IssuedAmount { get; set; }
    /// <summary>作廢金額</summary>
    public decimal VoidedAmount { get; set; }
}
