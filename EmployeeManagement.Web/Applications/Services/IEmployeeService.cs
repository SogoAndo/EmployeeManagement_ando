using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Services;

/// <summary>
/// 社員情報サービスインターフェイス
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// 社員一覧を取得する
    /// </summary>
    List<EmployeeEntity> GetEmployees();

    /// <summary>
    /// 指定されたIdの社員を取得する
    /// </summary>
    EmployeeEntity GetEmployeeById(int id);

    /// <summary>
    /// 社員を登録する
    /// </summary>
    void Register(EmployeeEntity employee);

    /// <summary>
    /// 社員を更新する
    /// </summary>
    void Update(EmployeeEntity employee);

    /// <summary>
    /// 社員を削除する
    /// </summary>
    void Delete(int id);
}
