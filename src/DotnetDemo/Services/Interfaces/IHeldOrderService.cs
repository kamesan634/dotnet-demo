using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 掛單服務介面
/// </summary>
public interface IHeldOrderService
{
    /// <summary>
    /// 取得門市的掛單列表
    /// </summary>
    Task<List<HeldOrder>> GetByStoreAsync(int storeId);

    /// <summary>
    /// 依編號取得掛單
    /// </summary>
    Task<HeldOrder?> GetByIdAsync(int id);

    /// <summary>
    /// 建立掛單
    /// </summary>
    Task<HeldOrder> CreateAsync(HeldOrder heldOrder);

    /// <summary>
    /// 刪除掛單 (取回結帳)
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 清理過期掛單
    /// </summary>
    Task<int> CleanupExpiredAsync();

    /// <summary>
    /// 產生掛單號碼
    /// </summary>
    Task<string> GenerateHoldNumberAsync();
}
