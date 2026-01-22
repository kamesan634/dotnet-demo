using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 操作紀錄服務介面
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// 依條件搜尋操作紀錄
    /// </summary>
    Task<List<AuditLog>> SearchAsync(int? userId, string? action, string? entityType, DateTime? startDate, DateTime? endDate, int limit = 100);

    /// <summary>
    /// 依編號取得操作紀錄
    /// </summary>
    Task<AuditLog?> GetByIdAsync(int id);

    /// <summary>
    /// 記錄操作
    /// </summary>
    Task<AuditLog> LogAsync(int? userId, string? userName, string action, string entityType, string? entityId, string? oldValues, string? newValues, string? ipAddress, string? userAgent);

    /// <summary>
    /// 取得實體的操作紀錄
    /// </summary>
    Task<List<AuditLog>> GetByEntityAsync(string entityType, string entityId);

    /// <summary>
    /// 取得使用者的操作紀錄
    /// </summary>
    Task<List<AuditLog>> GetByUserAsync(int userId, int limit = 50);
}
