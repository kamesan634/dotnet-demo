namespace DotnetDemo.Models.Entities;

/// <summary>
/// 會員等級
/// </summary>
public class CustomerLevel
{
    /// <summary>
    /// 等級編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 等級名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 升級所需累積消費金額
    /// </summary>
    public decimal RequiredAmount { get; set; }

    /// <summary>
    /// 折扣百分比
    /// </summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>
    /// 點數倍率
    /// </summary>
    public decimal PointMultiplier { get; set; } = 1;

    /// <summary>
    /// 排序 (越小等級越低)
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 此等級的會員
    /// </summary>
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
