using EmployeeManagement.Web.Applications.Filters;
using EmployeeManagement.Web.Applications.Security;
using EmployeeManagement.Web.Applications.Services;
using EmployeeManagement.Web.Infrastructures.Entities;
using EmployeeManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers;

/// <summary>
/// 部門機能を扱うController
/// </summary>
[LoginRequired]
public class DepartmentController : Controller
{
    private const string TempDataSuccessMessage = "SuccessMessage";

    private readonly IDepartmentService _departmentService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// 部門一覧画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Index(string? name)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var departments = _departmentService.GetDepartments();
        if (!string.IsNullOrWhiteSpace(name))
        {
            departments = departments
                .Where(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var viewModel = new DepartmentIndexViewModel
        {
            Name = name,
            Departments = departments.Select(ToListItemViewModel).ToList(),
            ReturnUrl = GetCurrentUrl()
        };

        return View(viewModel);
    }

    /// <summary>
    /// 部門詳細画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Details(int id, string? returnUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var department = _departmentService.GetDepartmentById(id);
        var viewModel = ToDetailViewModel(department);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        viewModel.CurrentUrl = GetCurrentUrl();
        return View(viewModel);
    }

    /// <summary>
    /// 部門登録画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Create(string? returnUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        return View(new DepartmentFormViewModel
        {
            ReturnUrl = NormalizeReturnUrl(returnUrl)
        });
    }

    /// <summary>
    /// 部門登録処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(DepartmentFormViewModel form)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        if (!ModelState.IsValid)
        {
            form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
            return View(form);
        }

        try
        {
            _departmentService.Register(new DepartmentEntity
            {
                Name = form.Name,
                CreatedByUserId = loginUserId.Value
            });
            TempData[TempDataSuccessMessage] = $"部門「{form.Name.Trim()}」を登録しました。";
            return RedirectToReturnUrlOrIndex(form.ReturnUrl);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
            return View(form);
        }
    }

    /// <summary>
    /// 部門更新画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Edit(int id, string? returnUrl, string? deleteReturnUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var department = _departmentService.GetDepartmentById(id);
        var viewModel = ToFormViewModel(department);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        viewModel.DeleteReturnUrl = NormalizeDeleteReturnUrl(
            deleteReturnUrl,
            returnUrl,
            Url.Action(nameof(Details), new { id }));
        return View(viewModel);
    }

    /// <summary>
    /// 部門更新処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(DepartmentFormViewModel form)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        if (!ModelState.IsValid)
        {
            NormalizeEditReturnUrls(form);
            return View(form);
        }

        try
        {
            _departmentService.Update(new DepartmentEntity
            {
                Id = form.Id,
                Name = form.Name,
                UpdatedByUserId = loginUserId.Value
            });
            TempData[TempDataSuccessMessage] = $"部門「{form.Name.Trim()}」を更新しました。";
            return RedirectToReturnUrlOrIndex(form.ReturnUrl);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            var department = _departmentService.GetDepartmentById(form.Id);
            form.CreatedByName = GetUserName(department.CreatedByUser);
            form.CreatedAtText = FormatDateTime(department.CreatedAt);
            form.UpdatedByName = GetUserName(department.UpdatedByUser);
            form.UpdatedAtText = FormatDateTime(department.UpdatedAt);
            NormalizeEditReturnUrls(form);
            return View(form);
        }
    }

    /// <summary>
    /// 部門削除確認画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Delete(int id, string? returnUrl, string? cancelUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var department = _departmentService.GetDepartmentById(id);
        var viewModel = ToDeleteViewModel(department);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        viewModel.CancelUrl = NormalizeReturnUrl(cancelUrl) ?? viewModel.ReturnUrl;
        return View(viewModel);
    }

    /// <summary>
    /// 部門削除処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id, string? returnUrl, string? cancelUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        try
        {
            var department = _departmentService.GetDepartmentById(id);
            _departmentService.Delete(id);
            TempData[TempDataSuccessMessage] = $"部門「{department.Name}」を削除しました。";
            return RedirectToReturnUrlOrIndex(returnUrl);
        }
        catch (Exception e)
        {
            var department = _departmentService.GetDepartmentById(id);
            var viewModel = ToDeleteViewModel(department);
            viewModel.ErrorMessage = e.Message;
            viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
            viewModel.CancelUrl = NormalizeReturnUrl(cancelUrl) ?? viewModel.ReturnUrl;
            return View("Delete", viewModel);
        }
    }

    private bool IsLoggedIn()
    {
        return GetLoginUserId() != null;
    }

    private int? GetLoginUserId()
    {
        return HttpContext.Session.GetInt32(SessionKeys.UserId);
    }

    private string GetCurrentUrl()
    {
        return $"{Request.Path}{Request.QueryString}";
    }

    private string? NormalizeReturnUrl(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : null;
    }

    private string? NormalizeDeleteReturnUrl(
        string? deleteReturnUrl,
        string? returnUrl,
        string? detailUrl)
    {
        var normalizedDeleteReturnUrl = NormalizeReturnUrl(deleteReturnUrl);
        if (normalizedDeleteReturnUrl != null)
        {
            return normalizedDeleteReturnUrl;
        }

        var normalizedReturnUrl = NormalizeReturnUrl(returnUrl);
        return IsSameLocalPath(normalizedReturnUrl, detailUrl) ? null : normalizedReturnUrl;
    }

    private static bool IsSameLocalPath(string? url, string? targetUrl)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(targetUrl))
        {
            return false;
        }

        var queryStart = url.IndexOf('?');
        var path = queryStart >= 0 ? url[..queryStart] : url;
        return string.Equals(
            path.TrimEnd('/'),
            targetUrl.TrimEnd('/'),
            StringComparison.OrdinalIgnoreCase);
    }

    private IActionResult RedirectToReturnUrlOrIndex(string? returnUrl)
    {
        var normalizedReturnUrl = NormalizeReturnUrl(returnUrl);
        return normalizedReturnUrl == null
            ? RedirectToAction(nameof(Index))
            : LocalRedirect(normalizedReturnUrl);
    }

    private static DepartmentListItemViewModel ToListItemViewModel(DepartmentEntity department)
    {
        return new DepartmentListItemViewModel
        {
            Id = department.Id,
            Name = department.Name,
            EmployeeCount = department.Employees.Count,
            UpdatedByName = GetUserName(department.UpdatedByUser ?? department.CreatedByUser),
            UpdatedAtText = FormatDate(department.UpdatedAt ?? department.CreatedAt)
        };
    }

    private static DepartmentFormViewModel ToFormViewModel(DepartmentEntity department)
    {
        return new DepartmentFormViewModel
        {
            Id = department.Id,
            Name = department.Name,
            CreatedByName = GetUserName(department.CreatedByUser),
            CreatedAtText = FormatDateTime(department.CreatedAt),
            UpdatedByName = GetUserName(department.UpdatedByUser),
            UpdatedAtText = FormatDateTime(department.UpdatedAt)
        };
    }

    private static DepartmentDetailViewModel ToDetailViewModel(DepartmentEntity department)
    {
        return new DepartmentDetailViewModel
        {
            Id = department.Id,
            Name = department.Name,
            EmployeeCount = department.Employees.Count,
            CreatedByName = GetUserName(department.CreatedByUser),
            CreatedAtText = FormatDateTime(department.CreatedAt),
            UpdatedByName = GetUserName(department.UpdatedByUser),
            UpdatedAtText = FormatDateTime(department.UpdatedAt),
            Employees = department.Employees
                .OrderBy(e => e.EmployeeNo)
                .Select(ToDepartmentEmployeeListItemViewModel)
                .ToList()
        };
    }

    private static DepartmentDeleteViewModel ToDeleteViewModel(DepartmentEntity department)
    {
        return new DepartmentDeleteViewModel
        {
            Id = department.Id,
            Name = department.Name,
            EmployeeCount = department.Employees.Count,
            UpdatedByName = GetUserName(department.UpdatedByUser ?? department.CreatedByUser),
            UpdatedAtText = FormatDate(department.UpdatedAt ?? department.CreatedAt)
        };
    }

    private static DepartmentEmployeeListItemViewModel ToDepartmentEmployeeListItemViewModel(
        EmployeeEntity employee)
    {
        return new DepartmentEmployeeListItemViewModel
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            Email = employee.Email ?? string.Empty,
            HireDateText = FormatDate(employee.HireDate)
        };
    }

    private static string GetUserName(LoginUserEntity? loginUser)
    {
        return loginUser?.Employee?.Name ?? string.Empty;
    }

    private void NormalizeEditReturnUrls(DepartmentFormViewModel form)
    {
        form.DeleteReturnUrl = NormalizeDeleteReturnUrl(
            form.DeleteReturnUrl,
            form.ReturnUrl,
            Url.Action(nameof(Details), new { id = form.Id }));
        form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
    }

    private static string FormatDate(DateTime? dateTime)
    {
        return dateTime?.ToString("yyyy/MM/dd") ?? string.Empty;
    }

    private static string FormatDateTime(DateTime? dateTime)
    {
        return dateTime?.ToString("yyyy/MM/dd HH:mm") ?? string.Empty;
    }
}
