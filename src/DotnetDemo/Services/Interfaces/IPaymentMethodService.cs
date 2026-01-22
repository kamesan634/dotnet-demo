using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 付款方式服務介面
/// </summary>
public interface IPaymentMethodService
{
    /// <summary>
    /// 取得所有付款方式
    /// </summary>
    Task<List<PaymentMethod>> GetAllAsync();

    /// <summary>
    /// 取得啟用中的付款方式
    /// </summary>
    Task<List<PaymentMethod>> GetActiveAsync();

    /// <summary>
    /// 依編號取得付款方式
    /// </summary>
    Task<PaymentMethod?> GetByIdAsync(int id);

    /// <summary>
    /// 新增付款方式
    /// </summary>
    Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod);

    /// <summary>
    /// 更新付款方式
    /// </summary>
    Task<bool> UpdateAsync(PaymentMethod paymentMethod);

    /// <summary>
    /// 刪除付款方式
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 檢查代碼是否存在
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}
