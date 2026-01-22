using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 操作紀錄服務實作
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditLogService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public AuditLogService(ApplicationDbContext context, ILogger<AuditLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> SearchAsync(int? userId, string? action, string? entityType, DateTime? startDate, DateTime? endDate, int limit = 100)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(l => l.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(l => l.Action.Contains(action));
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(l => l.EntityType == entityType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(l => l.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var end = endDate.Value.AddDays(1);
            query = query.Where(l => l.CreatedAt < end);
        }

        return await query.OrderByDescending(l => l.CreatedAt).Take(limit).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        return await _context.AuditLogs.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<AuditLog> LogAsync(int? userId, string? userName, string action, string entityType, string? entityId, string? oldValues, string? newValues, string? ipAddress, string? userAgent)
    {
        var log = new AuditLog
        {
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();

        return log;
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> GetByEntityAsync(string entityType, string entityId)
    {
        return await _context.AuditLogs
            .Where(l => l.EntityType == entityType && l.EntityId == entityId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> GetByUserAsync(int userId, int limit = 50)
    {
        return await _context.AuditLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
}
