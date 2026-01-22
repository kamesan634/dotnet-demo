using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 報表排程服務介面
/// </summary>
public interface IReportScheduleService
{
    /// <summary>
    /// 取得所有排程
    /// </summary>
    Task<List<ReportSchedule>> GetAllAsync();

    /// <summary>
    /// 依編號取得排程
    /// </summary>
    Task<ReportSchedule?> GetByIdAsync(int id);

    /// <summary>
    /// 取得待執行的排程
    /// </summary>
    Task<List<ReportSchedule>> GetPendingSchedulesAsync();

    /// <summary>
    /// 新增排程
    /// </summary>
    Task<ReportSchedule> CreateAsync(ReportSchedule schedule, int userId);

    /// <summary>
    /// 更新排程
    /// </summary>
    Task<bool> UpdateAsync(ReportSchedule schedule);

    /// <summary>
    /// 刪除排程
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 更新執行結果
    /// </summary>
    Task<bool> UpdateExecutionResultAsync(int id, bool success, string? error);

    /// <summary>
    /// 計算下次執行時間
    /// </summary>
    DateTime? CalculateNextExecution(ReportSchedule schedule);
}
