namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 社員詳細画面のViewModel
/// </summary>
public class EmployeeDetailViewModel
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

    /// <summary>
    /// 部門ID
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// 部門名
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// ログイン情報の登録・更新画面へ移動できるかどうか
    /// </summary>
    public bool CanManageLoginUser { get; set; }

    /// <summary>
    /// ログインユーザーが登録済みかどうか
    /// </summary>
    public bool HasLoginUser { get; set; }

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
    /// 一覧へ戻るURL
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 現在の社員詳細URL
    /// </summary>
    public string CurrentUrl { get; set; } = string.Empty;
}
