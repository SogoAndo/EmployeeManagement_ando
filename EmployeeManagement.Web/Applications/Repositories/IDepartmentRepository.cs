using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Repositories;

/// <summary>
/// 部門情報のCRUD操作インターフェイス
/// </summary>
public interface IDepartmentRepository
{
    /// <summary>
    /// すべての部門を取得する
    /// </summary>
    List<DepartmentEntity> FindAll();

    /// <summary>
    /// 引数Idに一致する部門を取得する
    /// </summary>
    DepartmentEntity? FindById(int id);

    /// <summary>
    /// 引数部門名に一致する部門を取得する
    /// </summary>
    DepartmentEntity? FindByName(string name);

    /// <summary>
    /// 引数部門名の存在有無を取得する
    /// </summary>
    bool ExistsByName(string name);

    /// <summary>
    /// 部門を登録する
    /// </summary>
    void Create(DepartmentEntity department);

    /// <summary>
    /// 部門を更新する
    /// </summary>
    void Update(DepartmentEntity department);

    /// <summary>
    /// 部門を削除する
    /// </summary>
    void Delete(DepartmentEntity department);
}
