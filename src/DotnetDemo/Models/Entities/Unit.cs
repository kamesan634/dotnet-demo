namespace DotnetDemo.Models.Entities;

/// <summary>
/// 計量單位
/// </summary>
public class Unit
{
    /// <summary>
    /// 單位編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 單位代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 單位名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 使用此單位的商品
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
