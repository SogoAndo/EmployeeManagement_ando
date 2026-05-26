namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 部門一覧に表示する1行分のViewModel
/// </summary>
public class DepartmentListItemViewModel
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
    /// 更新者名
    /// </summary>
    public string UpdatedByName { get; set; } = string.Empty;

    /// <summary>
    /// 更新日
    /// </summary>
    public string UpdatedAtText { get; set; } = string.Empty;
}
