using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 倉庫服務介面
/// </summary>
public interface IWarehouseService
{
    /// <summary>
    /// 取得所有倉庫
    /// </summary>
    Task<List<Warehouse>> GetAllAsync();

    /// <summary>
    /// 取得啟用中的倉庫
    /// </summary>
    Task<List<Warehouse>> GetActiveAsync();

    /// <summary>
    /// 依編號取得倉庫
    /// </summary>
    Task<Warehouse?> GetByIdAsync(int id);

    /// <summary>
    /// 新增倉庫
    /// </summary>
    Task<Warehouse> CreateAsync(Warehouse warehouse);

    /// <summary>
    /// 更新倉庫
    /// </summary>
    Task<bool> UpdateAsync(Warehouse warehouse);

    /// <summary>
    /// 刪除倉庫
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 檢查代碼是否存在
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);

    /// <summary>
    /// 取得門市下的倉庫
    /// </summary>
    Task<List<Warehouse>> GetByStoreAsync(int storeId);
}
