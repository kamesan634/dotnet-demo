using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 付款方式服務實作
/// </summary>
public class PaymentMethodService : IPaymentMethodService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PaymentMethodService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public PaymentMethodService(ApplicationDbContext context, ILogger<PaymentMethodService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<PaymentMethod>> GetAllAsync()
    {
        return await _context.PaymentMethods
            .OrderBy(p => p.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<PaymentMethod>> GetActiveAsync()
    {
        return await _context.PaymentMethods
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<PaymentMethod?> GetByIdAsync(int id)
    {
        return await _context.PaymentMethods.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod)
    {
        paymentMethod.CreatedAt = DateTime.UtcNow;
        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();

        _logger.LogInformation("付款方式 {Code} 已建立", paymentMethod.Code);
        return paymentMethod;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(PaymentMethod paymentMethod)
    {
        var existing = await _context.PaymentMethods.FindAsync(paymentMethod.Id);
        if (existing == null) return false;

        existing.Name = paymentMethod.Name;
        existing.RequiresChange = paymentMethod.RequiresChange;
        existing.SortOrder = paymentMethod.SortOrder;
        existing.IsActive = paymentMethod.IsActive;

        await _context.SaveChangesAsync();

        _logger.LogInformation("付款方式 {Code} 已更新", paymentMethod.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var paymentMethod = await _context.PaymentMethods.FindAsync(id);
        if (paymentMethod == null) return false;

        // 檢查是否有訂單使用此付款方式
        var hasOrders = await _context.OrderPayments.AnyAsync(op => op.PaymentMethodId == id);
        if (hasOrders)
        {
            _logger.LogWarning("付款方式 {Code} 有訂單使用中，無法刪除", paymentMethod.Code);
            return false;
        }

        _context.PaymentMethods.Remove(paymentMethod);
        await _context.SaveChangesAsync();

        _logger.LogInformation("付款方式 {Code} 已刪除", paymentMethod.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _context.PaymentMethods.Where(p => p.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }
}
