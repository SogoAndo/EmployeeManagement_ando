using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 社員登録・更新画面のViewModel
/// </summary>
public class EmployeeFormViewModel
{
    /// <summary>
    /// 社員ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 社員番号
    /// </summary>
    [Display(Name = "社員番号")]
    [Required(ErrorMessage = "{0}を入力してください。")]
    [StringLength(10, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "{0}は半角英数字で入力してください。")]
    [Remote(
        action: "IsEmployeeNoAvailable",
        controller: "Employee",
        AdditionalFields = nameof(Id),
        ErrorMessage = "この社員番号は既に使用されています。")]
    public string EmployeeNo { get; set; } = string.Empty;

    /// <summary>
    /// 社員名
    /// </summary>
    [Display(Name = "社員名")]
    [Required(ErrorMessage = "{0}を入力してください。")]
    [StringLength(50, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [RegularExpression(@".*\S.*", ErrorMessage = "{0}は空白だけでは登録できません。")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [Display(Name = "メールアドレス")]
    [StringLength(100, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "{0}はexample@example.comの形式で入力してください。")]
    public string? Email { get; set; }

    /// <summary>
    /// 入社日
    /// </summary>
    [Display(Name = "入社日")]
    [Required(ErrorMessage = "{0}を入力してください。")]
    [DataType(DataType.Date)]
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// 部門ID
    /// </summary>
    [Display(Name = "部門")]
    [Required(ErrorMessage = "{0}を選択してください。")]
    [Range(1, int.MaxValue, ErrorMessage = "{0}を選択してください。")]
    public int? DepartmentId { get; set; }

    /// <summary>
    /// 部門選択肢
    /// </summary>
    public List<SelectListItem> DepartmentOptions { get; set; } = new();

    /// <summary>
    /// 保存後またはキャンセル時に戻るURL
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 削除後に戻るURL
    /// </summary>
    public string? DeleteReturnUrl { get; set; }

    /// <summary>
    /// ログインユーザーが登録済みかどうか
    /// </summary>
    public bool HasLoginUser { get; set; }

    /// <summary>
    /// ログイン情報の登録・更新画面へ移動できるかどうか
    /// </summary>
    public bool CanManageLoginUser { get; set; }

    /// <summary>
    /// 登録者名
    /// </summary>
    public string CreatedByName { get; set; } = "保存時に設定";

    /// <summary>
    /// 登録日時
    /// </summary>
    public string CreatedAtText { get; set; } = "保存時に設定";

    /// <summary>
    /// 更新者名
    /// </summary>
    public string UpdatedByName { get; set; } = "更新時に設定";

    /// <summary>
    /// 更新日時
    /// </summary>
    public string UpdatedAtText { get; set; } = "更新時に設定";
}
