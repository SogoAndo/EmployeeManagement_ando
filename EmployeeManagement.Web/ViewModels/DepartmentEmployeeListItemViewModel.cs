namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 部門詳細画面に表示する所属社員1行分のViewModel
/// </summary>
public class DepartmentEmployeeListItemViewModel
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
    /// メールアドレス
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 入社日
    /// </summary>
    public string HireDateText { get; set; } = string.Empty;
}
