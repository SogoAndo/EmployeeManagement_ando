namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 社員ログイン情報削除確認画面のViewModel
/// </summary>
public class EmployeeLoginUserDeleteViewModel
{
    /// <summary>
    /// 社員ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// 社員番号
    /// </summary>
    public string EmployeeNo { get; set; } = string.Empty;

    /// <summary>
    /// 社員名
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// 部門名
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// ログインID
    /// </summary>
    public string LoginId { get; set; } = string.Empty;

    /// <summary>
    /// 削除不可などのメッセージ
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 削除後に戻るURL
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 削除をキャンセルした時に戻るURL
    /// </summary>
    public string? CancelUrl { get; set; }
}
