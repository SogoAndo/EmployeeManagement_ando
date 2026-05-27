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
    /// 社員とログイン情報を登録する
    /// </summary>
    void Register(EmployeeEntity employee, string? loginId, string? password);

    /// <summary>
    /// 社員を更新する
    /// </summary>
    void Update(EmployeeEntity employee);

    /// <summary>
    /// 社員とログイン情報を更新する
    /// </summary>
    void Update(EmployeeEntity employee, string? loginId, string? password);

    /// <summary>
    /// 社員のログイン情報を登録または更新する
    /// </summary>
    void SaveLoginUser(int employeeId, string? loginId, string? password);

    /// <summary>
    /// 指定された社員IDに紐づくログインユーザーを取得する
    /// </summary>
    LoginUserEntity? GetLoginUserByEmployeeId(int employeeId);

    /// <summary>
    /// ログインIDが指定された社員で利用可能か判定する
    /// </summary>
    bool IsLoginIdAvailable(string? loginId, int employeeId);

    /// <summary>
    /// 社員のログイン情報を削除する
    /// </summary>
    void DeleteLoginUser(int employeeId);

    /// <summary>
    /// 社員を削除する
    /// </summary>
    void Delete(int id);
}
