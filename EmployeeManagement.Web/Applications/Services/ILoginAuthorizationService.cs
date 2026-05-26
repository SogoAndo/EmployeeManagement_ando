using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Services;

/// <summary>
/// ログイン可否判定サービスインターフェイス
/// </summary>
public interface ILoginAuthorizationService
{
    /// <summary>
    /// ログインユーザーがログイン可能か判定する
    /// </summary>
    bool CanLogin(LoginUserEntity loginUser);
}
