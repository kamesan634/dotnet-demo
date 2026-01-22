using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 編號規則服務實作
/// </summary>
public class NumberingRuleService : INumberingRuleService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public NumberingRuleService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<NumberingRule>> GetAllAsync()
    {
        return await _context.NumberingRules
            .OrderBy(x => x.DocumentType)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<NumberingRule?> GetByIdAsync(int id)
    {
        return await _context.NumberingRules.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<NumberingRule?> GetByDocumentTypeAsync(string documentType)
    {
        return await _context.NumberingRules
            .FirstOrDefaultAsync(x => x.DocumentType == documentType && x.IsActive);
    }

    /// <inheritdoc/>
    public async Task<string> GenerateNextNumberAsync(string documentType)
    {
        var rule = await GetByDocumentTypeAsync(documentType);
        if (rule == null)
        {
            return $"{documentType}{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }

        var now = DateTime.Now;
        var shouldReset = ShouldResetSequence(rule, now);

        if (shouldReset)
        {
            rule.CurrentSequence = 0;
            rule.LastResetDate = now;
        }

        rule.CurrentSequence++;
        await _context.SaveChangesAsync();

        var dateFormat = string.IsNullOrEmpty(rule.DateFormat) ? "" : now.ToString(rule.DateFormat);
        var sequence = rule.CurrentSequence.ToString().PadLeft(rule.SequenceLength, '0');

        return $"{rule.Prefix}{dateFormat}{sequence}";
    }

    /// <inheritdoc/>
    public async Task<NumberingRule> CreateAsync(NumberingRule rule)
    {
        _context.NumberingRules.Add(rule);
        await _context.SaveChangesAsync();
        return rule;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(NumberingRule rule)
    {
        var existing = await _context.NumberingRules.FindAsync(rule.Id);
        if (existing == null) return false;

        existing.Name = rule.Name;
        existing.Prefix = rule.Prefix;
        existing.DateFormat = rule.DateFormat;
        existing.SequenceLength = rule.SequenceLength;
        existing.ResetPeriod = rule.ResetPeriod;
        existing.IsActive = rule.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var rule = await _context.NumberingRules.FindAsync(id);
        if (rule == null)
            return (false, "編號規則不存在");

        _context.NumberingRules.Remove(rule);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private bool ShouldResetSequence(NumberingRule rule, DateTime now)
    {
        if (rule.LastResetDate == null) return false;

        return rule.ResetPeriod switch
        {
            "Daily" => rule.LastResetDate.Value.Date < now.Date,
            "Monthly" => rule.LastResetDate.Value.Year < now.Year ||
                        rule.LastResetDate.Value.Month < now.Month,
            "Yearly" => rule.LastResetDate.Value.Year < now.Year,
            _ => false
        };
    }
}
