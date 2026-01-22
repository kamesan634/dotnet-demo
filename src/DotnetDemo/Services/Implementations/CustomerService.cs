using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 客戶服務實作
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ApplicationDbContext context, ILogger<CustomerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.CustomerLevel)
            .OrderBy(c => c.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.CustomerLevel)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc />
    public async Task<Customer> CreateAsync(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow;
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("客戶 {Code} 已建立", customer.Code);
        return customer;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Customer customer)
    {
        var existing = await _context.Customers.FindAsync(customer.Id);
        if (existing == null) return false;

        existing.Name = customer.Name;
        existing.Phone = customer.Phone;
        existing.Email = customer.Email;
        existing.Address = customer.Address;
        existing.Birthday = customer.Birthday;
        existing.CustomerLevelId = customer.CustomerLevelId;
        existing.Notes = customer.Notes;
        existing.IsActive = customer.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("客戶 {Code} 已更新", customer.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("客戶 {Code} 已刪除", customer.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePointsAsync(int id, int points)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        customer.Points += points;
        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("客戶 {Id} 點數更新: {Points}", id, points);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateTotalSpentAsync(int id, decimal amount)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        customer.TotalSpent += amount;
        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("客戶 {Id} 累積消費更新: {Amount}", id, amount);
        return true;
    }

    /// <inheritdoc />
    public async Task<List<Customer>> GetActiveAsync()
    {
        return await _context.Customers
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
