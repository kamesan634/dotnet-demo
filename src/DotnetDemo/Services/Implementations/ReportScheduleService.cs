using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 報表排程服務實作
/// </summary>
public class ReportScheduleService : IReportScheduleService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public ReportScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<ReportSchedule>> GetAllAsync()
    {
        return await _context.ReportSchedules
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ReportSchedule?> GetByIdAsync(int id)
    {
        return await _context.ReportSchedules.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<List<ReportSchedule>> GetPendingSchedulesAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.ReportSchedules
            .Where(x => x.IsActive)
            .Where(x => x.NextExecutionAt != null && x.NextExecutionAt <= now)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ReportSchedule> CreateAsync(ReportSchedule schedule, int userId)
    {
        schedule.CreatedBy = userId;
        schedule.NextExecutionAt = CalculateNextExecution(schedule);

        _context.ReportSchedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(ReportSchedule schedule)
    {
        var existing = await _context.ReportSchedules.FindAsync(schedule.Id);
        if (existing == null) return false;

        existing.Name = schedule.Name;
        existing.ReportType = schedule.ReportType;
        existing.Frequency = schedule.Frequency;
        existing.ExecutionTime = schedule.ExecutionTime;
        existing.DayOfWeek = schedule.DayOfWeek;
        existing.DayOfMonth = schedule.DayOfMonth;
        existing.Parameters = schedule.Parameters;
        existing.Recipients = schedule.Recipients;
        existing.ExportFormat = schedule.ExportFormat;
        existing.IsActive = schedule.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.NextExecutionAt = CalculateNextExecution(existing);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var schedule = await _context.ReportSchedules.FindAsync(id);
        if (schedule == null) return false;

        _context.ReportSchedules.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateExecutionResultAsync(int id, bool success, string? error)
    {
        var schedule = await _context.ReportSchedules.FindAsync(id);
        if (schedule == null) return false;

        schedule.LastExecutedAt = DateTime.UtcNow;
        schedule.LastExecutionResult = success ? "Success" : $"Failed: {error}";
        schedule.NextExecutionAt = CalculateNextExecution(schedule);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public DateTime? CalculateNextExecution(ReportSchedule schedule)
    {
        if (!schedule.IsActive) return null;

        var now = DateTime.Now;
        var timeParts = schedule.ExecutionTime.Split(':');
        var hour = int.Parse(timeParts[0]);
        var minute = timeParts.Length > 1 ? int.Parse(timeParts[1]) : 0;

        DateTime nextRun;

        switch (schedule.Frequency)
        {
            case ReportFrequency.Daily:
                nextRun = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
                if (nextRun <= now)
                    nextRun = nextRun.AddDays(1);
                break;

            case ReportFrequency.Weekly:
                var targetDayOfWeek = schedule.DayOfWeek ?? 1;
                nextRun = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
                var daysUntilTarget = ((targetDayOfWeek - (int)now.DayOfWeek) + 7) % 7;
                if (daysUntilTarget == 0 && nextRun <= now)
                    daysUntilTarget = 7;
                nextRun = nextRun.AddDays(daysUntilTarget);
                break;

            case ReportFrequency.Monthly:
                var targetDay = schedule.DayOfMonth ?? 1;
                var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                var actualDay = Math.Min(targetDay, daysInMonth);
                nextRun = new DateTime(now.Year, now.Month, actualDay, hour, minute, 0);
                if (nextRun <= now)
                {
                    var nextMonth = now.AddMonths(1);
                    daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                    actualDay = Math.Min(targetDay, daysInMonth);
                    nextRun = new DateTime(nextMonth.Year, nextMonth.Month, actualDay, hour, minute, 0);
                }
                break;

            default:
                return null;
        }

        return nextRun;
    }
}
