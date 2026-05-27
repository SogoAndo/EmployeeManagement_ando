using EmployeeManagement.Web.Applications.Security;
using EmployeeManagement.Web.Applications.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.Web.Applications.Filters;

/// <summary>
/// ログイン中かつログイン可能なユーザーであることを確認するフィルター属性
/// </summary>
public class LoginRequiredAttribute : TypeFilterAttribute
{
    public LoginRequiredAttribute() : base(typeof(LoginRequiredFilter)) { }
}

/// <summary>
/// セッションのログインユーザーをリクエストごとに再検証するフィルター
/// </summary>
public class LoginRequiredFilter : IActionFilter
{
    private readonly ILoginService _loginService;
    private readonly ILoginAuthorizationService _loginAuthorizationService;

    public LoginRequiredFilter(
        ILoginService loginService,
        ILoginAuthorizationService loginAuthorizationService)
    {
        _loginService = loginService;
        _loginAuthorizationService = loginAuthorizationService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var loginUserId = context.HttpContext.Session.GetInt32(SessionKeys.UserId);
        if (loginUserId == null)
        {
            RedirectToLogin(context);
            return;
        }

        try
        {
            var loginUser = _loginService.GetLoginUserById(loginUserId.Value);
            if (_loginAuthorizationService.CanLogin(loginUser))
            {
                return;
            }
        }
        catch (KeyNotFoundException)
        {
            // 削除済みユーザーのセッションはログイン画面へ戻す。
        }

        context.HttpContext.Session.Clear();
        RedirectToLogin(context);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private static void RedirectToLogin(ActionExecutingContext context)
    {
        context.Result = new RedirectToActionResult("Index", "Login", null);
    }
}
