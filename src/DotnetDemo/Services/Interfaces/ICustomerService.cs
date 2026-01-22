using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 客戶服務介面
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// 取得所有客戶
    /// </summary>
    Task<List<Customer>> GetAllAsync();

    /// <summary>
    /// 依編號取得客戶
    /// </summary>
    Task<Customer?> GetByIdAsync(int id);

    /// <summary>
    /// 新增客戶
    /// </summary>
    Task<Customer> CreateAsync(Customer customer);

    /// <summary>
    /// 更新客戶
    /// </summary>
    Task<bool> UpdateAsync(Customer customer);

    /// <summary>
    /// 刪除客戶
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 更新客戶點數
    /// </summary>
    Task<bool> UpdatePointsAsync(int id, int points);

    /// <summary>
    /// 更新客戶累積消費
    /// </summary>
    Task<bool> UpdateTotalSpentAsync(int id, decimal amount);

    /// <summary>
    /// 取得啟用中的客戶
    /// </summary>
    Task<List<Customer>> GetActiveAsync();
}
