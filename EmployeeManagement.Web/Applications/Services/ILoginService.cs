using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Services;

/// <summary>
/// ログインサービスインターフェイス
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// ログインIDとパスワードで認証する
    /// </summary>
    LoginUserEntity? Authenticate(string loginId, string password);

    /// <summary>
    /// 指定されたIdのログインユーザーを取得する
    /// </summary>
    LoginUserEntity GetLoginUserById(int id);
}
