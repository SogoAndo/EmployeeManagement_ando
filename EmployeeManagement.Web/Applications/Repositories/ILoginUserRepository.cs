using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Repositories;

/// <summary>
/// ログインユーザー情報の操作インターフェイス
/// </summary>
public interface ILoginUserRepository
{
    /// <summary>
    /// 引数Idに一致するログインユーザーを取得する
    /// </summary>
    LoginUserEntity? FindById(int id);

    /// <summary>
    /// 引数ログインIDに一致するログインユーザーを取得する
    /// </summary>
    LoginUserEntity? FindByLoginId(string loginId);

    /// <summary>
    /// 引数社員IDに一致するログインユーザーを取得する
    /// </summary>
    LoginUserEntity? FindByEmployeeId(int employeeId);

    /// <summary>
    /// 引数ログインIDの存在有無を取得する
    /// </summary>
    bool ExistsByLoginId(string loginId);

    /// <summary>
    /// ログインユーザーを登録する
    /// </summary>
    void Create(LoginUserEntity loginUser);

    /// <summary>
    /// ログインユーザーを更新する
    /// </summary>
    void Update(LoginUserEntity loginUser);

    /// <summary>
    /// ログインユーザーを削除する
    /// </summary>
    void Delete(LoginUserEntity loginUser);
}
