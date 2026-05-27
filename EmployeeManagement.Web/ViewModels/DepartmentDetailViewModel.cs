namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 部門詳細画面のViewModel
/// </summary>
public class DepartmentDetailViewModel
{
    /// <summary>
    /// 部門ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 部門名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 所属社員数
    /// </summary>
    public int EmployeeCount { get; set; }

    /// <summary>
    /// 登録者名
    /// </summary>
    public string CreatedByName { get; set; } = string.Empty;

    /// <summary>
    /// 登録日時
    /// </summary>
    public string CreatedAtText { get; set; } = string.Empty;

    /// <summary>
    /// 更新者名
    /// </summary>
    public string UpdatedByName { get; set; } = string.Empty;

    /// <summary>
    /// 更新日時
    /// </summary>
    public string UpdatedAtText { get; set; } = string.Empty;

    /// <summary>
    /// 所属社員一覧
    /// </summary>
    public List<DepartmentEmployeeListItemViewModel> Employees { get; set; } = new();

    /// <summary>
    /// 一覧へ戻るURL
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 現在の部門詳細URL
    /// </summary>
    public string CurrentUrl { get; set; } = string.Empty;
}
