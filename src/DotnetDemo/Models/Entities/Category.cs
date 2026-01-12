namespace DotnetDemo.Models.Entities;

/// <summary>
/// 商品分類
/// </summary>
public class Category
{
    /// <summary>
    /// 分類編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 分類代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 分類名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父分類編號
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 父分類
    /// </summary>
    public Category? Parent { get; set; }

    /// <summary>
    /// 子分類
    /// </summary>
    public ICollection<Category> Children { get; set; } = new List<Category>();

    /// <summary>
    /// 層級
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 分類下的商品
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
