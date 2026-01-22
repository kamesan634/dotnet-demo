using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 發票服務實作
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public InvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<Invoice>> SearchAsync(string? keyword, InvoiceType? type, InvoiceStatus? status, DateTime? from, DateTime? to)
    {
        var query = _context.Invoices
            .Include(x => x.Order)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.InvoiceNumber.Contains(keyword) ||
                                    (x.BuyerName != null && x.BuyerName.Contains(keyword)) ||
                                    (x.BuyerTaxId != null && x.BuyerTaxId.Contains(keyword)));
        }

        if (type.HasValue)
            query = query.Where(x => x.Type == type.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (from.HasValue)
            query = query.Where(x => x.IssuedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.IssuedAt <= to.Value);

        return await query.OrderByDescending(x => x.IssuedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices
            .Include(x => x.Order)
                .ThenInclude(o => o.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<Invoice?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(x => x.OrderId == orderId && x.Status != InvoiceStatus.Voided);
    }

    /// <inheritdoc/>
    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        if (string.IsNullOrEmpty(invoice.InvoiceNumber))
        {
            invoice.InvoiceNumber = await GenerateInvoiceNumberAsync();
        }

        invoice.RandomNumber = GenerateRandomNumber();
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> VoidAsync(int id, string reason, int userId)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
            return (false, "發票不存在");

        if (invoice.Status == InvoiceStatus.Voided)
            return (false, "發票已作廢");

        invoice.Status = InvoiceStatus.Voided;
        invoice.VoidedAt = DateTime.UtcNow;
        invoice.VoidReason = reason;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    /// <inheritdoc/>
    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var prefix = GetInvoicePrefix();
        var today = DateTime.Today;
        var count = await _context.Invoices
            .CountAsync(x => x.InvoiceNumber.StartsWith(prefix) &&
                            x.IssuedAt >= today);

        return $"{prefix}{(count + 1).ToString().PadLeft(8, '0')}";
    }

    /// <inheritdoc/>
    public async Task<InvoiceStats> GetStatsAsync(DateTime from, DateTime to)
    {
        var invoices = await _context.Invoices
            .Where(x => x.IssuedAt >= from && x.IssuedAt <= to)
            .ToListAsync();

        var issued = invoices.Where(x => x.Status != InvoiceStatus.Voided).ToList();
        var voided = invoices.Where(x => x.Status == InvoiceStatus.Voided).ToList();

        return new InvoiceStats
        {
            IssuedCount = issued.Count,
            VoidedCount = voided.Count,
            IssuedAmount = issued.Sum(x => x.TotalAmount),
            VoidedAmount = voided.Sum(x => x.TotalAmount)
        };
    }

    private string GetInvoicePrefix()
    {
        var now = DateTime.Now;
        var monthCode = ((now.Month - 1) / 2) switch
        {
            0 => "AB",
            1 => "CD",
            2 => "EF",
            3 => "GH",
            4 => "IJ",
            5 => "KL",
            _ => "XX"
        };
        return monthCode;
    }

    private string GenerateRandomNumber()
    {
        var random = new Random();
        return random.Next(1000, 9999).ToString();
    }
}
