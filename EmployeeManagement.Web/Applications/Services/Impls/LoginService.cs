using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Services.Impls;

/// <summary>
/// ログインサービスインターフェイスの実装
/// </summary>
public class LoginService : ILoginService
{
    private readonly ILoginUserRepository _loginUserRepository;
    private readonly ILoginAuthorizationService _loginAuthorizationService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LoginService(
        ILoginUserRepository loginUserRepository,
        ILoginAuthorizationService loginAuthorizationService)
    {
        _loginUserRepository = loginUserRepository;
        _loginAuthorizationService = loginAuthorizationService;
    }

    /// <summary>
    /// ログインIDとパスワードで認証する
    /// </summary>
    public LoginUserEntity? Authenticate(string loginId, string password)
    {
        if (string.IsNullOrWhiteSpace(loginId) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var loginUser = _loginUserRepository.FindByLoginId(loginId);
        if (loginUser == null)
        {
            return null;
        }

        if (loginUser.Password != password)
        {
            return null;
        }

        if (!_loginAuthorizationService.CanLogin(loginUser))
        {
            return null;
        }

        return loginUser;
    }

    /// <summary>
    /// 指定されたIdのログインユーザーを取得する
    /// </summary>
    public LoginUserEntity GetLoginUserById(int id)
    {
        var loginUser = _loginUserRepository.FindById(id);
        if (loginUser == null)
        {
            throw new KeyNotFoundException($"指定されたログインユーザーID:{id}は存在しません。");
        }
        return loginUser;
    }
}
