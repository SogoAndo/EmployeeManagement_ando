using EmployeeManagement.Web.Applications.Security;
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
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) != null)
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
            ClearPasswordField(form);
            return View(form);
        }

        var loginUser = _loginService.Authenticate(form.LoginId, form.Password);
        if (loginUser == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "ログインIDまたはパスワードが正しくありません。人事部所属者のみログインできます。");
            ClearPasswordField(form);
            return View(form);
        }

        HttpContext.Session.SetInt32(SessionKeys.UserId, loginUser.Id);
        HttpContext.Session.SetString(SessionKeys.LoginId, loginUser.LoginId);
        HttpContext.Session.SetString(SessionKeys.UserName, loginUser.Employee?.Name ?? string.Empty);

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

    private void ClearPasswordField(LoginViewModel form)
    {
        form.Password = string.Empty;
        ClearModelStateValue(nameof(LoginViewModel.Password));
    }

    private void ClearModelStateValue(string key)
    {
        if (!ModelState.TryGetValue(key, out var state))
        {
            return;
        }

        var errorMessages = state.Errors
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToList();

        ModelState.Remove(key);
        foreach (var errorMessage in errorMessages)
        {
            ModelState.AddModelError(key, errorMessage!);
        }
    }
}
