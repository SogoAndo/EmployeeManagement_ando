using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Web.Infrastructures.Entities;

/// <summary>
/// 社員テーブル(employees)を扱うEntity Framework Coreのエンティティクラス
/// </summary>
[Table("employees")]
public class EmployeeEntity
{
    /// <summary>
    /// 社員ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 社員番号
    /// </summary>
    [Required]
    [StringLength(10)]
    [Column("employee_no")]
    public string EmployeeNo { get; set; } = string.Empty;

    /// <summary>
    /// 社員名
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    [Column("email")]
    public string? Email { get; set; }

    /// <summary>
    /// 入社日
    /// </summary>
    [Required]
    [Column("hire_date")]
    public DateTime HireDate { get; set; }

    /// <summary>
    /// 部門ID
    /// </summary>
    [Required]
    [Column("department_id")]
    public int DepartmentId { get; set; }

    /// <summary>
    /// 登録者ID
    /// </summary>
    [Required]
    [Column("created_by_user_id")]
    public int CreatedByUserId { get; set; }

    /// <summary>
    /// 登録日時
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新日時
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新者ID
    /// </summary>
    [Column("updated_by_user_id")]
    public int? UpdatedByUserId { get; set; }

    /// <summary>
    /// 所属部門
    /// </summary>
    public DepartmentEntity? Department { get; set; }

    /// <summary>
    /// 登録したログインユーザー
    /// </summary>
    public LoginUserEntity? CreatedByUser { get; set; }

    /// <summary>
    /// 最後に更新したログインユーザー
    /// </summary>
    public LoginUserEntity? UpdatedByUser { get; set; }
}
