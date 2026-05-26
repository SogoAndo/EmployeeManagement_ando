using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Web.Infrastructures.Entities;

/// <summary>
/// 部門テーブル(departments)を扱うEntity Framework Coreのエンティティクラス
/// </summary>
[Table("departments")]
public class DepartmentEntity
{
    /// <summary>
    /// 部門ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 部門名
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

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
    /// 部門に所属する社員
    /// </summary>
    public List<EmployeeEntity> Employees { get; set; } = new();

    /// <summary>
    /// 登録したログインユーザー
    /// </summary>
    public LoginUserEntity? CreatedByUser { get; set; }

    /// <summary>
    /// 最後に更新したログインユーザー
    /// </summary>
    public LoginUserEntity? UpdatedByUser { get; set; }
}
