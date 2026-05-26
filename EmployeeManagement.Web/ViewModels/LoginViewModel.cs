using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// ログイン画面の入力値を保持するViewModel
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// ログインID
    /// </summary>
    [Display(Name = "ログインID")]
    [Required(ErrorMessage = "{0}を入力してください。")]
    [StringLength(50, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "{0}は4桁の数字で入力してください。")]
    public string LoginId { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Display(Name = "パスワード")]
    [Required(ErrorMessage = "{0}を入力してください。")]
    [StringLength(255, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
