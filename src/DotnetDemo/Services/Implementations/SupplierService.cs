using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 供應商服務實作
/// </summary>
public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(ApplicationDbContext context, ILogger<SupplierService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .OrderBy(s => s.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Supplier>> GetActiveAsync()
    {
        return await _context.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        supplier.CreatedAt = DateTime.UtcNow;
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        _logger.LogInformation("供應商 {Code} 已建立", supplier.Code);
        return supplier;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Supplier supplier)
    {
        var existing = await _context.Suppliers.FindAsync(supplier.Id);
        if (existing == null) return false;

        existing.Name = supplier.Name;
        existing.ContactName = supplier.ContactName;
        existing.Phone = supplier.Phone;
        existing.Email = supplier.Email;
        existing.Address = supplier.Address;
        existing.TaxId = supplier.TaxId;
        existing.PaymentTermDays = supplier.PaymentTermDays;
        existing.Notes = supplier.Notes;
        existing.IsActive = supplier.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("供應商 {Code} 已更新", supplier.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return false;

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        _logger.LogInformation("供應商 {Code} 已刪除", supplier.Code);
        return true;
    }
}
