using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Web.ViewModels;

/// <summary>
/// 部門登録・更新画面のViewModel
/// </summary>
public class DepartmentFormViewModel
{
    /// <summary>
    /// 部門ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 部門名
    /// </summary>
    [Display(Name = "部門名")]
    [Required(ErrorMessage = "{0}を入力してください。")]
    [StringLength(50, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    [RegularExpression(@".*\S.*", ErrorMessage = "{0}は空白だけでは登録できません。")]
    public string Name { get; set; } = string.Empty;

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
