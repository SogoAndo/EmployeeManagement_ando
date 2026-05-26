using EmployeeManagement.Web.Applications.Services;
using EmployeeManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers;

/// <summary>
/// ログイン/ログアウトを扱うController
/// </summary>
[Route("Login")]
public class LoginController : Controller
{
    private const string SessionLoginUserId = "LoginUserId";
    private const string SessionLoginId = "LoginId";
    private const string SessionEmployeeName = "EmployeeName";

    private readonly ILoginService _loginService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    /// <summary>
    /// ログイン画面を表示する
    /// </summary>
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32(SessionLoginUserId) != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    /// <summary>
    /// ログイン処理を行う
    /// </summary>
    [HttpPost("Index")]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoginViewModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var loginUser = _loginService.Authenticate(form.LoginId, form.Password);
        if (loginUser == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "ログインIDまたはパスワードが正しくありません。人事部所属者のみログインできます。");
            return View(form);
        }

        HttpContext.Session.SetInt32(SessionLoginUserId, loginUser.Id);
        HttpContext.Session.SetString(SessionLoginId, loginUser.LoginId);
        HttpContext.Session.SetString(SessionEmployeeName, loginUser.Employee?.Name ?? string.Empty);

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// ログアウト処理を行う
    /// </summary>
    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Login");
    }
}
