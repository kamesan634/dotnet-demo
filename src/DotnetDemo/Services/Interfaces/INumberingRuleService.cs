using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 編號規則服務介面
/// </summary>
public interface INumberingRuleService
{
    /// <summary>
    /// 取得所有編號規則
    /// </summary>
    Task<List<NumberingRule>> GetAllAsync();

    /// <summary>
    /// 依編號取得規則
    /// </summary>
    Task<NumberingRule?> GetByIdAsync(int id);

    /// <summary>
    /// 依單據類型取得規則
    /// </summary>
    Task<NumberingRule?> GetByDocumentTypeAsync(string documentType);

    /// <summary>
    /// 產生下一個編號
    /// </summary>
    Task<string> GenerateNextNumberAsync(string documentType);

    /// <summary>
    /// 新增規則
    /// </summary>
    Task<NumberingRule> CreateAsync(NumberingRule rule);

    /// <summary>
    /// 更新規則
    /// </summary>
    Task<bool> UpdateAsync(NumberingRule rule);

    /// <summary>
    /// 刪除規則
    /// </summary>
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
