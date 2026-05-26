namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 部門一覧画面のViewModel
/// </summary>
public class DepartmentIndexViewModel
{
    /// <summary>
    /// 検索条件: 部門名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 部門一覧
    /// </summary>
    public List<DepartmentListItemViewModel> Departments { get; set; } = new();
}
