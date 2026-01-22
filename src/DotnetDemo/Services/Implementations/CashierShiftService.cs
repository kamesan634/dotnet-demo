using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 收銀班別服務實作
/// </summary>
public class CashierShiftService : ICashierShiftService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public CashierShiftService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<CashierShift>> SearchAsync(int? storeId, int? cashierId, CashierShiftStatus? status, DateTime? from, DateTime? to)
    {
        var query = _context.CashierShifts
            .Include(x => x.Store)
            .Include(x => x.Cashier)
            .AsQueryable();

        if (storeId.HasValue)
            query = query.Where(x => x.StoreId == storeId.Value);

        if (cashierId.HasValue)
            query = query.Where(x => x.CashierId == cashierId.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (from.HasValue)
            query = query.Where(x => x.OpenedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.OpenedAt <= to.Value);

        return await query.OrderByDescending(x => x.OpenedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<CashierShift?> GetByIdAsync(int id)
    {
        return await _context.CashierShifts
            .Include(x => x.Store)
            .Include(x => x.Cashier)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<CashierShift?> GetCurrentShiftAsync(int cashierId)
    {
        return await _context.CashierShifts
            .Include(x => x.Store)
            .FirstOrDefaultAsync(x => x.CashierId == cashierId && x.Status == CashierShiftStatus.Open);
    }

    /// <inheritdoc/>
    public async Task<(CashierShift? Shift, string? Error)> OpenShiftAsync(int storeId, int cashierId, decimal openingAmount)
    {
        var existingShift = await GetCurrentShiftAsync(cashierId);
        if (existingShift != null)
            return (null, "此收銀員已有進行中的班別，請先結班");

        var shift = new CashierShift
        {
            StoreId = storeId,
            CashierId = cashierId,
            OpenedAt = DateTime.UtcNow,
            OpeningAmount = openingAmount,
            Status = CashierShiftStatus.Open
        };

        _context.CashierShifts.Add(shift);
        await _context.SaveChangesAsync();

        return (shift, null);
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> CloseShiftAsync(int shiftId, decimal closingAmount, string? notes)
    {
        var shift = await _context.CashierShifts.FindAsync(shiftId);
        if (shift == null)
            return (false, "班別不存在");

        if (shift.Status != CashierShiftStatus.Open)
            return (false, "此班別已結班");

        var expectedAmount = shift.OpeningAmount + shift.CashSalesTotal;
        shift.ClosingAmount = closingAmount;
        shift.ClosedAt = DateTime.UtcNow;
        shift.Difference = closingAmount - expectedAmount;
        shift.Notes = notes;
        shift.Status = CashierShiftStatus.Closed;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateSalesStatsAsync(int shiftId, decimal cashAmount, decimal nonCashAmount)
    {
        var shift = await _context.CashierShifts.FindAsync(shiftId);
        if (shift == null || shift.Status != CashierShiftStatus.Open)
            return false;

        shift.CashSalesTotal += cashAmount;
        shift.NonCashSalesTotal += nonCashAmount;
        shift.TransactionCount++;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<ShiftSummary> GetShiftSummaryAsync(int shiftId)
    {
        var shift = await GetByIdAsync(shiftId);
        if (shift == null)
            return new ShiftSummary();

        var orders = await _context.Orders
            .Where(o => o.CreatedBy == shift.CashierId &&
                       o.CreatedAt >= shift.OpenedAt &&
                       (shift.ClosedAt == null || o.CreatedAt <= shift.ClosedAt))
            .Include(o => o.Payments)
            .ToListAsync();

        var refundAmount = orders
            .Where(o => o.Status == OrderStatus.Refunded)
            .Sum(o => o.TotalAmount);

        return new ShiftSummary
        {
            TransactionCount = shift.TransactionCount,
            CashSales = shift.CashSalesTotal,
            NonCashSales = shift.NonCashSalesTotal,
            TotalSales = shift.CashSalesTotal + shift.NonCashSalesTotal,
            RefundAmount = refundAmount,
            NetSales = shift.CashSalesTotal + shift.NonCashSalesTotal - refundAmount
        };
    }
}
