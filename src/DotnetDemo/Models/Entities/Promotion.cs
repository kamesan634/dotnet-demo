namespace DotnetDemo.Models.Entities;

/// <summary>
/// 促銷類型
/// </summary>
public enum PromotionType
{
    /// <summary>折扣</summary>
    Discount = 1,
    /// <summary>滿額折</summary>
    AmountOff = 2,
    /// <summary>買X送Y</summary>
    BuyXGetY = 3,
    /// <summary>組合價</summary>
    Bundle = 4,
    /// <summary>會員專屬</summary>
    MemberOnly = 5
}

/// <summary>
/// 促銷活動
/// </summary>
public class Promotion
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 活動名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 活動代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 活動說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 促銷類型
    /// </summary>
    public PromotionType Type { get; set; }

    /// <summary>
    /// 折扣值 (折扣%或折扣金額)
    /// </summary>
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// 最低消費金額
    /// </summary>
    public decimal? MinimumAmount { get; set; }

    /// <summary>
    /// 最低購買數量
    /// </summary>
    public int? MinimumQuantity { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 適用門市編號 (null 表示全部門市)
    /// </summary>
    public int? StoreId { get; set; }

    /// <summary>
    /// 適用門市
    /// </summary>
    public Store? Store { get; set; }

    /// <summary>
    /// 適用會員等級編號 (null 表示全部會員)
    /// </summary>
    public int? CustomerLevelId { get; set; }

    /// <summary>
    /// 適用會員等級
    /// </summary>
    public CustomerLevel? CustomerLevel { get; set; }

    /// <summary>
    /// 使用次數限制 (null 表示無限制)
    /// </summary>
    public int? UsageLimit { get; set; }

    /// <summary>
    /// 已使用次數
    /// </summary>
    public int UsageCount { get; set; }

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
    /// 促銷商品
    /// </summary>
    public ICollection<PromotionItem> Items { get; set; } = new List<PromotionItem>();
}
