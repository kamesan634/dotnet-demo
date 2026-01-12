using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 庫存服務實作
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(ApplicationDbContext context, ILogger<InventoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Inventory>> GetAllAsync()
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.Product.IsActive)
            .OrderBy(i => i.Product.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Inventory>> SearchAsync(string? searchText, int? warehouseId, bool lowStockOnly)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.Product.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(i =>
                i.Product.Name.Contains(searchText) ||
                i.Product.Code.Contains(searchText));
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == warehouseId.Value);
        }

        if (lowStockOnly)
        {
            query = query.Where(i => i.Quantity < i.Product.SafetyStock);
        }

        return await query.OrderBy(i => i.Product.Code).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Inventory>> GetLowStockAsync()
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.Product.IsActive && i.Quantity < i.Product.SafetyStock)
            .OrderBy(i => i.Quantity)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId);
    }

    /// <inheritdoc />
    public async Task<bool> AdjustQuantityAsync(int productId, int warehouseId, int quantity, string reason, int userId)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId);

        if (inventory == null)
        {
            inventory = new Inventory
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                Quantity = 0
            };
            _context.Inventories.Add(inventory);
        }

        var beforeQuantity = inventory.Quantity;
        inventory.Quantity += quantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        // 記錄異動
        var movement = new InventoryMovement
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            Type = MovementType.Adjustment,
            Quantity = quantity,
            BeforeQuantity = beforeQuantity,
            AfterQuantity = inventory.Quantity,
            Notes = reason,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        _context.InventoryMovements.Add(movement);

        await _context.SaveChangesAsync();

        _logger.LogInformation("庫存調整: 商品 {ProductId}, 倉庫 {WarehouseId}, 數量 {Quantity}", productId, warehouseId, quantity);
        return true;
    }

    /// <inheritdoc />
    public async Task<List<InventoryMovement>> GetMovementsAsync(int productId, int? warehouseId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.InventoryMovements
            .Include(m => m.Product)
            .Include(m => m.Warehouse)
            .Where(m => m.ProductId == productId)
            .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(m => m.WarehouseId == warehouseId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt <= endDate.Value);
        }

        return await query.OrderByDescending(m => m.CreatedAt).ToListAsync();
    }
}
