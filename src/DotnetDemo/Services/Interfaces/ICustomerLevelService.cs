using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 會員等級服務介面
/// </summary>
public interface ICustomerLevelService
{
    /// <summary>
    /// 取得所有會員等級
    /// </summary>
    Task<List<CustomerLevel>> GetAllAsync();

    /// <summary>
    /// 取得啟用中的會員等級
    /// </summary>
    Task<List<CustomerLevel>> GetActiveAsync();

    /// <summary>
    /// 依編號取得會員等級
    /// </summary>
    Task<CustomerLevel?> GetByIdAsync(int id);

    /// <summary>
    /// 新增會員等級
    /// </summary>
    Task<CustomerLevel> CreateAsync(CustomerLevel level);

    /// <summary>
    /// 更新會員等級
    /// </summary>
    Task<bool> UpdateAsync(CustomerLevel level);

    /// <summary>
    /// 刪除會員等級
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 依累積消費金額取得對應等級
    /// </summary>
    Task<CustomerLevel?> GetLevelByAmountAsync(decimal amount);
}
