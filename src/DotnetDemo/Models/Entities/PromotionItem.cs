namespace DotnetDemo.Models.Entities;

/// <summary>
/// 促銷商品
/// </summary>
public class PromotionItem
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 促銷活動編號
    /// </summary>
    public int PromotionId { get; set; }

    /// <summary>
    /// 促銷活動
    /// </summary>
    public Promotion Promotion { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 促銷價格 (如有指定)
    /// </summary>
    public decimal? PromotionPrice { get; set; }

    /// <summary>
    /// 最大購買數量
    /// </summary>
    public int? MaxQuantity { get; set; }
}
