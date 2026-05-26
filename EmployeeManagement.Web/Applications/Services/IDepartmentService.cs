using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Services;

/// <summary>
/// 部門情報サービスインターフェイス
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// 部門一覧を取得する
    /// </summary>
    List<DepartmentEntity> GetDepartments();

    /// <summary>
    /// 指定されたIdの部門を取得する
    /// </summary>
    DepartmentEntity GetDepartmentById(int id);

    /// <summary>
    /// 部門を登録する
    /// </summary>
    void Register(DepartmentEntity department);

    /// <summary>
    /// 部門を更新する
    /// </summary>
    void Update(DepartmentEntity department);

    /// <summary>
    /// 部門を削除する
    /// </summary>
    void Delete(int id);
}
