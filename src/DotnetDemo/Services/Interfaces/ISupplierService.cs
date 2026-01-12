using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 供應商服務介面
/// </summary>
public interface ISupplierService
{
    /// <summary>
    /// 取得所有供應商
    /// </summary>
    Task<List<Supplier>> GetAllAsync();

    /// <summary>
    /// 取得啟用的供應商
    /// </summary>
    Task<List<Supplier>> GetActiveAsync();

    /// <summary>
    /// 依編號取得供應商
    /// </summary>
    Task<Supplier?> GetByIdAsync(int id);

    /// <summary>
    /// 新增供應商
    /// </summary>
    Task<Supplier> CreateAsync(Supplier supplier);

    /// <summary>
    /// 更新供應商
    /// </summary>
    Task<bool> UpdateAsync(Supplier supplier);

    /// <summary>
    /// 刪除供應商
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
