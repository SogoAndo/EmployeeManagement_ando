namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 社員一覧に表示する1行分のViewModel
/// </summary>
public class EmployeeListItemViewModel
{
    /// <summary>
    /// 社員ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 社員番号
    /// </summary>
    public string EmployeeNo { get; set; } = string.Empty;

    /// <summary>
    /// 社員名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部門名
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 入社日
    /// </summary>
    public string HireDateText { get; set; } = string.Empty;

    /// <summary>
    /// 更新者名
    /// </summary>
    public string UpdatedByName { get; set; } = string.Empty;

    /// <summary>
    /// 更新日
    /// </summary>
    public string UpdatedAtText { get; set; } = string.Empty;
}
