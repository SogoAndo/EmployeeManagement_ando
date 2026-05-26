using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Services.Impls;

/// <summary>
/// ログイン可否判定サービスインターフェイスの実装
/// </summary>
public class LoginAuthorizationService : ILoginAuthorizationService
{
    private const string HumanResourcesDepartmentName = "人事部";

    /// <summary>
    /// ログインユーザーがログイン可能か判定する
    /// </summary>
    public bool CanLogin(LoginUserEntity loginUser)
    {
        return loginUser.Employee?.Department?.Name == HumanResourcesDepartmentName;
    }
}
