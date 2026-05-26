using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Web.Infrastructures.Entities;

/// <summary>
/// ログインユーザーテーブル(users)を扱うEntity Framework Coreのエンティティクラス
/// </summary>
[Table("users")]
public class LoginUserEntity
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// ログインID
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("login_id")]
    public string LoginId { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 社員ID
    /// </summary>
    [Required]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    /// <summary>
    /// ログインユーザーに紐づく社員
    /// </summary>
    public EmployeeEntity? Employee { get; set; }
}
