using EmployeeManagement.Web.Applications.Services;
using EmployeeManagement.Web.Infrastructures.Entities;
using EmployeeManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Web.Controllers;

/// <summary>
/// 社員機能を扱うController
/// </summary>
public class EmployeeController : Controller
{
    private const string SessionLoginUserId = "LoginUserId";

    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EmployeeController(
        IEmployeeService employeeService,
        IDepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    /// <summary>
    /// 社員一覧画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Index(string? employeeNo, string? name, int? departmentId)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var employees = _employeeService.GetEmployees();
        if (!string.IsNullOrWhiteSpace(employeeNo))
        {
            employees = employees
                .Where(e => e.EmployeeNo.Contains(employeeNo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        if (!string.IsNullOrWhiteSpace(name))
        {
            employees = employees
                .Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        if (departmentId != null)
        {
            employees = employees
                .Where(e => e.DepartmentId == departmentId.Value)
                .ToList();
        }

        var viewModel = new EmployeeIndexViewModel
        {
            EmployeeNo = employeeNo,
            Name = name,
            DepartmentId = departmentId,
            DepartmentOptions = CreateDepartmentOptions(true),
            Employees = employees.Select(ToListItemViewModel).ToList()
        };

        return View(viewModel);
    }

    /// <summary>
    /// 社員詳細画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Details(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        return View(ToDetailViewModel(employee));
    }

    /// <summary>
    /// 社員登録画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        return View(new EmployeeFormViewModel
        {
            HireDate = DateTime.Today,
            DepartmentOptions = CreateDepartmentOptions()
        });
    }

    /// <summary>
    /// 社員登録処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EmployeeFormViewModel form)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        if (!ModelState.IsValid)
        {
            form.DepartmentOptions = CreateDepartmentOptions();
            return View(form);
        }

        try
        {
            _employeeService.Register(new EmployeeEntity
            {
                EmployeeNo = form.EmployeeNo.Trim(),
                Name = form.Name.Trim(),
                Email = NormalizeEmail(form.Email),
                HireDate = form.HireDate!.Value,
                DepartmentId = form.DepartmentId!.Value,
                CreatedByUserId = loginUserId.Value
            });
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            form.DepartmentOptions = CreateDepartmentOptions();
            return View(form);
        }
    }

    /// <summary>
    /// 社員更新画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        var viewModel = ToFormViewModel(employee);
        viewModel.DepartmentOptions = CreateDepartmentOptions();
        return View(viewModel);
    }

    /// <summary>
    /// 社員更新処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EmployeeFormViewModel form)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        if (!ModelState.IsValid)
        {
            FillEditSupportData(form);
            return View(form);
        }

        try
        {
            _employeeService.Update(new EmployeeEntity
            {
                Id = form.Id,
                EmployeeNo = form.EmployeeNo.Trim(),
                Name = form.Name.Trim(),
                Email = NormalizeEmail(form.Email),
                HireDate = form.HireDate!.Value,
                DepartmentId = form.DepartmentId!.Value,
                UpdatedByUserId = loginUserId.Value
            });
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            FillEditSupportData(form);
            return View(form);
        }
    }

    /// <summary>
    /// 社員削除確認画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        return View(ToDeleteViewModel(employee));
    }

    /// <summary>
    /// 社員削除処理を行う
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
            _employeeService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            var employee = _employeeService.GetEmployeeById(id);
            var viewModel = ToDeleteViewModel(employee);
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

    private List<SelectListItem> CreateDepartmentOptions(bool includeEmptyOption = false)
    {
        var options = _departmentService.GetDepartments()
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            })
            .ToList();

        options.Insert(0, new SelectListItem
        {
            Value = string.Empty,
            Text = includeEmptyOption ? "すべて" : "選択してください"
        });

        return options;
    }

    private void FillEditSupportData(EmployeeFormViewModel form)
    {
        form.DepartmentOptions = CreateDepartmentOptions();

        if (form.Id <= 0)
        {
            return;
        }

        var employee = _employeeService.GetEmployeeById(form.Id);
        form.CreatedByName = GetUserName(employee.CreatedByUser);
        form.CreatedAtText = FormatDateTime(employee.CreatedAt);
        form.UpdatedByName = GetUserName(employee.UpdatedByUser);
        form.UpdatedAtText = FormatDateTime(employee.UpdatedAt);
    }

    private static EmployeeListItemViewModel ToListItemViewModel(EmployeeEntity employee)
    {
        return new EmployeeListItemViewModel
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            DepartmentName = employee.Department?.Name ?? string.Empty,
            Email = employee.Email ?? string.Empty,
            HireDateText = FormatDate(employee.HireDate),
            UpdatedByName = GetUserName(employee.UpdatedByUser ?? employee.CreatedByUser),
            UpdatedAtText = FormatDate(employee.UpdatedAt ?? employee.CreatedAt)
        };
    }

    private static EmployeeDetailViewModel ToDetailViewModel(EmployeeEntity employee)
    {
        return new EmployeeDetailViewModel
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            Email = employee.Email ?? string.Empty,
            HireDateText = FormatDate(employee.HireDate),
            DepartmentName = employee.Department?.Name ?? string.Empty,
            CreatedByName = GetUserName(employee.CreatedByUser),
            CreatedAtText = FormatDateTime(employee.CreatedAt),
            UpdatedByName = GetUserName(employee.UpdatedByUser),
            UpdatedAtText = FormatDateTime(employee.UpdatedAt)
        };
    }

    private static EmployeeFormViewModel ToFormViewModel(EmployeeEntity employee)
    {
        return new EmployeeFormViewModel
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            Email = employee.Email,
            HireDate = employee.HireDate,
            DepartmentId = employee.DepartmentId,
            CreatedByName = GetUserName(employee.CreatedByUser),
            CreatedAtText = FormatDateTime(employee.CreatedAt),
            UpdatedByName = GetUserName(employee.UpdatedByUser),
            UpdatedAtText = FormatDateTime(employee.UpdatedAt)
        };
    }

    private static EmployeeDeleteViewModel ToDeleteViewModel(EmployeeEntity employee)
    {
        return new EmployeeDeleteViewModel
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            DepartmentName = employee.Department?.Name ?? string.Empty
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

    private static string? NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }
}
