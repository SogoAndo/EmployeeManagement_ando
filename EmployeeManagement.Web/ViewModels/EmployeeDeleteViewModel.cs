namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 社員削除確認画面のViewModel
/// </summary>
public class EmployeeDeleteViewModel
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
    /// 削除不可などのメッセージ
    /// </summary>
    public string? ErrorMessage { get; set; }
}
