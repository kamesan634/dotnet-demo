namespace DotnetDemo.Models.Entities;

/// <summary>
/// 系統參數
/// </summary>
public class SystemSetting
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 參數鍵值
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 參數值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 參數說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 參數類型 (String, Int, Bool, Json)
    /// </summary>
    public string DataType { get; set; } = "String";

    /// <summary>
    /// 群組
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否可編輯
    /// </summary>
    public bool IsEditable { get; set; } = true;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新人員編號
    /// </summary>
    public int? UpdatedBy { get; set; }
}
