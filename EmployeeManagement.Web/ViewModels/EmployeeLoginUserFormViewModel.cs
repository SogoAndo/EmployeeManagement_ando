using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 社員ログイン情報登録・更新画面のViewModel
/// </summary>
public class EmployeeLoginUserFormViewModel
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
    /// ログインユーザーが登録済みかどうか
    /// </summary>
    public bool HasLoginUser { get; set; }

    /// <summary>
    /// ログインID
    /// </summary>
    [Display(Name = "ログインID")]
    [Required(ErrorMessage = "{0}を入力してください。")]
    [StringLength(50, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "{0}は4桁の数字で入力してください。")]
    [Remote(
        action: "IsLoginIdAvailable",
        controller: "Employee",
        AdditionalFields = nameof(EmployeeId),
        ErrorMessage = "このログインIDは既に使用されています。")]
    public string? LoginId { get; set; }

    /// <summary>
    /// パスワード
    /// </summary>
    [Display(Name = "パスワード")]
    [StringLength(255, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    /// <summary>
    /// パスワード確認
    /// </summary>
    [Display(Name = "パスワード確認")]
    [StringLength(255, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "パスワードとパスワード確認が一致しません。")]
    public string? ConfirmPassword { get; set; }

    /// <summary>
    /// 保存後またはキャンセル時に戻るURL
    /// </summary>
    public string? ReturnUrl { get; set; }
}
