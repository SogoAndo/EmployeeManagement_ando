using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 社員一覧画面のViewModel
/// </summary>
public class EmployeeIndexViewModel
{
    /// <summary>
    /// 検索条件: 社員番号
    /// </summary>
    public string? EmployeeNo { get; set; }

    /// <summary>
    /// 検索条件: 社員名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 検索条件: 部門ID
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// 検索条件: 部門名
    /// </summary>
    public string? SelectedDepartmentName { get; set; }

    /// <summary>
    /// 部門選択肢
    /// </summary>
    public List<SelectListItem> DepartmentOptions { get; set; } = new();

    /// <summary>
    /// 社員一覧
    /// </summary>
    public List<EmployeeListItemViewModel> Employees { get; set; } = new();

    /// <summary>
    /// 現在の一覧URL
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// 呼び出し元へ戻るURL
    /// </summary>
    public string? BackUrl { get; set; }
}
