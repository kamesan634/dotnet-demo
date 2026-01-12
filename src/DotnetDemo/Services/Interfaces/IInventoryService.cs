using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 庫存服務介面
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// 取得所有庫存
    /// </summary>
    Task<List<Inventory>> GetAllAsync();

    /// <summary>
    /// 依條件搜尋庫存
    /// </summary>
    Task<List<Inventory>> SearchAsync(string? searchText, int? warehouseId, bool lowStockOnly);

    /// <summary>
    /// 取得低庫存商品
    /// </summary>
    Task<List<Inventory>> GetLowStockAsync();

    /// <summary>
    /// 依商品和倉庫取得庫存
    /// </summary>
    Task<Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId);

    /// <summary>
    /// 調整庫存
    /// </summary>
    Task<bool> AdjustQuantityAsync(int productId, int warehouseId, int quantity, string reason, int userId);

    /// <summary>
    /// 取得庫存異動記錄
    /// </summary>
    Task<List<InventoryMovement>> GetMovementsAsync(int productId, int? warehouseId, DateTime? startDate, DateTime? endDate);
}
