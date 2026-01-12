namespace DotnetDemo.Models.Entities;

/// <summary>
/// 倉庫
/// </summary>
public class Warehouse
{
    /// <summary>
    /// 倉庫編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 倉庫代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 倉庫名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 所屬門市編號
    /// </summary>
    public int? StoreId { get; set; }

    /// <summary>
    /// 所屬門市
    /// </summary>
    public Store? Store { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 是否為主倉庫
    /// </summary>
    public bool IsPrimary { get; set; }

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
    /// 庫存記錄
    /// </summary>
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
