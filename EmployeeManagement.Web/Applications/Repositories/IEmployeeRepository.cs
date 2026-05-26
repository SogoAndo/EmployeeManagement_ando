using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Repositories;

/// <summary>
/// 社員情報のCRUD操作インターフェイス
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// すべての社員を取得する
    /// </summary>
    List<EmployeeEntity> FindAll();

    /// <summary>
    /// 引数Idに一致する社員を取得する
    /// </summary>
    EmployeeEntity? FindById(int id);

    /// <summary>
    /// 引数社員番号に一致する社員を取得する
    /// </summary>
    EmployeeEntity? FindByEmployeeNo(string employeeNo);

    /// <summary>
    /// 引数社員番号の存在有無を取得する
    /// </summary>
    bool ExistsByEmployeeNo(string employeeNo);

    /// <summary>
    /// 社員を登録する
    /// </summary>
    void Create(EmployeeEntity employee);

    /// <summary>
    /// 社員を更新する
    /// </summary>
    void Update(EmployeeEntity employee);

    /// <summary>
    /// 社員を削除する
    /// </summary>
    void Delete(EmployeeEntity employee);
}
