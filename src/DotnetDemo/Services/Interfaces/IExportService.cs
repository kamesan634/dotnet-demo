namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 匯出服務介面
/// </summary>
public interface IExportService
{
    /// <summary>
    /// 匯出資料為 Excel
    /// </summary>
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, Dictionary<string, string>? columnMappings = null);

    /// <summary>
    /// 匯出資料為 CSV
    /// </summary>
    Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, Dictionary<string, string>? columnMappings = null);

    /// <summary>
    /// 匯出銷售報表
    /// </summary>
    Task<byte[]> ExportSalesReportAsync(DateTime from, DateTime to);

    /// <summary>
    /// 匯出庫存報表
    /// </summary>
    Task<byte[]> ExportInventoryReportAsync(int? warehouseId = null);

    /// <summary>
    /// 匯出採購報表
    /// </summary>
    Task<byte[]> ExportPurchasingReportAsync(DateTime from, DateTime to);
}
