using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 分類服務介面
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// 取得所有分類
    /// </summary>
    Task<List<Category>> GetAllAsync();

    /// <summary>
    /// 取得啟用的分類
    /// </summary>
    Task<List<Category>> GetActiveAsync();

    /// <summary>
    /// 依編號取得分類
    /// </summary>
    Task<Category?> GetByIdAsync(int id);

    /// <summary>
    /// 新增分類
    /// </summary>
    Task<Category> CreateAsync(Category category);

    /// <summary>
    /// 更新分類
    /// </summary>
    Task<bool> UpdateAsync(Category category);

    /// <summary>
    /// 刪除分類
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 檢查分類是否可刪除
    /// </summary>
    Task<bool> CanDeleteAsync(int id);
}
