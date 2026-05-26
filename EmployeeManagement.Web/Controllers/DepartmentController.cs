using EmployeeManagement.Web.Applications.Services;
using EmployeeManagement.Web.Infrastructures.Entities;
using EmployeeManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers;

/// <summary>
/// 部門機能を扱うController
/// </summary>
public class DepartmentController : Controller
{
    private const string SessionLoginUserId = "LoginUserId";

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
            Departments = departments.Select(ToListItemViewModel).ToList()
        };

        return View(viewModel);
    }

    /// <summary>
    /// 部門登録画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        return View(new DepartmentFormViewModel());
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
            return View(form);
        }

        try
        {
            _departmentService.Register(new DepartmentEntity
            {
                Name = form.Name,
                CreatedByUserId = loginUserId.Value
            });
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            return View(form);
        }
    }

    /// <summary>
    /// 部門更新画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var department = _departmentService.GetDepartmentById(id);
        return View(ToFormViewModel(department));
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
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            var department = _departmentService.GetDepartmentById(form.Id);
            form.CreatedByName = GetUserName(department.CreatedByUser);
            form.CreatedAtText = FormatDateTime(department.CreatedAt);
            form.UpdatedByName = GetUserName(department.UpdatedByUser);
            form.UpdatedAtText = FormatDateTime(department.UpdatedAt);
            return View(form);
        }
    }

    /// <summary>
    /// 部門削除確認画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var department = _departmentService.GetDepartmentById(id);
        return View(ToDeleteViewModel(department));
    }

    /// <summary>
    /// 部門削除処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        try
        {
            _departmentService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            var department = _departmentService.GetDepartmentById(id);
            var viewModel = ToDeleteViewModel(department);
            viewModel.ErrorMessage = e.Message;
            return View("Delete", viewModel);
        }
    }

    private bool IsLoggedIn()
    {
        return GetLoginUserId() != null;
    }

    private int? GetLoginUserId()
    {
        return HttpContext.Session.GetInt32(SessionLoginUserId);
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

    private static string GetUserName(LoginUserEntity? loginUser)
    {
        return loginUser?.Employee?.Name ?? string.Empty;
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
