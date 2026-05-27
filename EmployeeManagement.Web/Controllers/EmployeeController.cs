using EmployeeManagement.Web.Applications.Filters;
using EmployeeManagement.Web.Applications.Security;
using EmployeeManagement.Web.Applications.Services;
using EmployeeManagement.Web.Infrastructures.Entities;
using EmployeeManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Web.Controllers;

/// <summary>
/// 社員機能を扱うController
/// </summary>
[LoginRequired]
public class EmployeeController : Controller
{
    private const string HumanResourcesDepartmentName = "人事部";
    private const string TempDataSuccessMessage = "SuccessMessage";
    private const string TempDataErrorMessage = "ErrorMessage";

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
    public IActionResult Index(
        string? employeeNo,
        string? name,
        int? departmentId,
        string? returnUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var departments = _departmentService.GetDepartments();
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
            SelectedDepartmentName = departments.FirstOrDefault(d => d.Id == departmentId)?.Name,
            DepartmentOptions = CreateDepartmentOptions(departments, true),
            Employees = employees.Select(ToListItemViewModel).ToList(),
            ReturnUrl = GetCurrentUrl(),
            BackUrl = NormalizeReturnUrl(returnUrl)
        };

        return View(viewModel);
    }

    /// <summary>
    /// 社員詳細画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Details(int id, string? returnUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        var viewModel = ToDetailViewModel(employee);
        var loginUser = _employeeService.GetLoginUserByEmployeeId(employee.Id);
        viewModel.HasLoginUser = loginUser != null;
        viewModel.CanManageLoginUser = CanManageLoginUser(employee, loginUser);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        viewModel.CurrentUrl = GetCurrentUrl();
        return View(viewModel);
    }

    /// <summary>
    /// 社員登録画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Create(string? returnUrl, int? departmentId)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var viewModel = new EmployeeFormViewModel
        {
            HireDate = DateTime.Today,
            DepartmentId = departmentId,
            ReturnUrl = NormalizeReturnUrl(returnUrl)
        };
        FillFormSupportData(viewModel);

        return View(viewModel);
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
            form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
            FillFormSupportData(form);
            return View(form);
        }

        try
        {
            var employee = new EmployeeEntity
            {
                EmployeeNo = form.EmployeeNo.Trim(),
                Name = form.Name.Trim(),
                Email = NormalizeEmail(form.Email),
                HireDate = form.HireDate!.Value,
                DepartmentId = form.DepartmentId!.Value,
                CreatedByUserId = loginUserId.Value
            };
            _employeeService.Register(employee);
            TempData[TempDataSuccessMessage] = $"社員「{employee.Name}」を登録しました。";
            return RedirectToReturnUrlOrIndex(form.ReturnUrl);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
            FillFormSupportData(form);
            return View(form);
        }
    }

    /// <summary>
    /// 社員番号が利用可能か検証する
    /// </summary>
    [HttpGet]
    public IActionResult IsEmployeeNoAvailable(string? employeeNo, int? id)
    {
        if (string.IsNullOrWhiteSpace(employeeNo))
        {
            return Json(true);
        }

        var normalizedEmployeeNo = employeeNo.Trim();
        var sameEmployee = _employeeService.GetEmployees()
            .FirstOrDefault(e => e.EmployeeNo == normalizedEmployeeNo);

        if (sameEmployee == null || sameEmployee.Id == id.GetValueOrDefault())
        {
            return Json(true);
        }

        return Json($"社員番号:{normalizedEmployeeNo}は既に使用されています。");
    }

    /// <summary>
    /// 社員更新画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Edit(int id, string? returnUrl, string? deleteReturnUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        var viewModel = ToFormViewModel(employee);
        FillFormSupportData(viewModel);
        FillLoginUserSummaryData(viewModel, employee);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        viewModel.DeleteReturnUrl = NormalizeDeleteReturnUrl(
            deleteReturnUrl,
            returnUrl,
            Url.Action(nameof(Details), new { id }));
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
            NormalizeEditReturnUrls(form);
            FillEditSupportData(form);
            return View(form);
        }

        try
        {
            var employee = new EmployeeEntity
            {
                Id = form.Id,
                EmployeeNo = form.EmployeeNo.Trim(),
                Name = form.Name.Trim(),
                Email = NormalizeEmail(form.Email),
                HireDate = form.HireDate!.Value,
                DepartmentId = form.DepartmentId!.Value,
                UpdatedByUserId = loginUserId.Value
            };
            _employeeService.Update(employee);
            TempData[TempDataSuccessMessage] = $"社員「{employee.Name}」を更新しました。";
            return RedirectToReturnUrlOrIndex(form.ReturnUrl);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            NormalizeEditReturnUrls(form);
            FillEditSupportData(form);
            return View(form);
        }
    }

    /// <summary>
    /// 社員ログイン情報登録・更新画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult LoginUser(int id, string? returnUrl)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        var loginUser = _employeeService.GetLoginUserByEmployeeId(employee.Id);
        if (!IsHumanResourcesEmployee(employee))
        {
            TempData[TempDataErrorMessage] = "ログイン情報は人事部の社員にのみ設定できます。";
            return RedirectToAction(nameof(Details), new { id, returnUrl });
        }

        if (!CanManageLoginUser(employee, loginUser, loginUserId.Value))
        {
            TempData[TempDataErrorMessage] = "他のユーザーのログイン情報は編集できません。";
            return RedirectToAction(nameof(Details), new { id, returnUrl });
        }

        var viewModel = ToLoginUserFormViewModel(employee, loginUser);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        return View(viewModel);
    }

    /// <summary>
    /// 社員ログイン情報登録・更新処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LoginUser(EmployeeLoginUserFormViewModel form)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(form.EmployeeId);
        var loginUser = _employeeService.GetLoginUserByEmployeeId(employee.Id);
        if (!IsHumanResourcesEmployee(employee))
        {
            ModelState.AddModelError(string.Empty, "ログイン情報は人事部の社員にのみ設定できます。");
        }
        else if (!CanManageLoginUser(employee, loginUser, loginUserId.Value))
        {
            TempData[TempDataErrorMessage] = "他のユーザーのログイン情報は編集できません。";
            return RedirectToReturnUrlOrDetails(form.ReturnUrl, form.EmployeeId);
        }

        FillLoginUserFormSupportData(form, employee, loginUser, overwriteLoginId: false);
        ValidatePasswordInput(form);

        if (!ModelState.IsValid)
        {
            form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
            ClearPasswordFields(form);
            return View(form);
        }

        try
        {
            _employeeService.SaveLoginUser(form.EmployeeId, form.LoginId, form.Password);
            TempData[TempDataSuccessMessage] = $"社員「{employee.Name}」のログイン情報を保存しました。";
            return RedirectToReturnUrlOrDetails(form.ReturnUrl, form.EmployeeId);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            FillLoginUserFormSupportData(form, employee, loginUser, overwriteLoginId: false);
            form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
            ClearPasswordFields(form);
            return View(form);
        }
    }

    /// <summary>
    /// ログインIDが利用可能か検証する
    /// </summary>
    [HttpGet]
    public IActionResult IsLoginIdAvailable(string? loginId, int? employeeId)
    {
        if (string.IsNullOrWhiteSpace(loginId))
        {
            return Json(true);
        }

        var normalizedLoginId = loginId.Trim();
        if (_employeeService.IsLoginIdAvailable(normalizedLoginId, employeeId.GetValueOrDefault()))
        {
            return Json(true);
        }

        return Json($"ログインID:{normalizedLoginId}は既に使用されています。");
    }

    /// <summary>
    /// 社員ログイン情報削除確認画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult DeleteLoginUser(int id, string? returnUrl, string? cancelUrl)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        var loginUser = _employeeService.GetLoginUserByEmployeeId(employee.Id);
        if (loginUser == null)
        {
            TempData[TempDataErrorMessage] = "削除対象のログイン情報は登録されていません。";
            return RedirectToAction(nameof(Details), new { id, returnUrl });
        }

        if (!CanManageLoginUser(employee, loginUser, loginUserId.Value))
        {
            TempData[TempDataErrorMessage] = "他のユーザーのログイン情報は削除できません。";
            return RedirectToAction(nameof(Details), new { id, returnUrl });
        }

        var viewModel = ToLoginUserDeleteViewModel(employee, loginUser);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        viewModel.CancelUrl = NormalizeReturnUrl(cancelUrl)
            ?? viewModel.ReturnUrl
            ?? Url.Action(nameof(Details), new { id });

        if (loginUserId == loginUser.Id)
        {
            viewModel.ErrorMessage = "現在ログイン中のユーザー自身のログイン情報は削除できません。";
        }

        return View(viewModel);
    }

    /// <summary>
    /// 社員ログイン情報削除処理を行う
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteLoginUserConfirmed(int id, string? returnUrl, string? cancelUrl)
    {
        var loginUserId = GetLoginUserId();
        if (loginUserId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        var loginUser = _employeeService.GetLoginUserByEmployeeId(employee.Id);
        if (loginUser == null)
        {
            TempData[TempDataErrorMessage] = "削除対象のログイン情報は登録されていません。";
            return RedirectToReturnUrlOrDetails(returnUrl, id);
        }

        if (!CanManageLoginUser(employee, loginUser, loginUserId.Value))
        {
            TempData[TempDataErrorMessage] = "他のユーザーのログイン情報は削除できません。";
            return RedirectToReturnUrlOrDetails(returnUrl, id);
        }

        if (loginUserId == loginUser.Id)
        {
            var viewModel = ToLoginUserDeleteViewModel(employee, loginUser);
            viewModel.ErrorMessage = "現在ログイン中のユーザー自身のログイン情報は削除できません。";
            viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
            viewModel.CancelUrl = NormalizeReturnUrl(cancelUrl) ?? viewModel.ReturnUrl;
            return View("DeleteLoginUser", viewModel);
        }

        try
        {
            _employeeService.DeleteLoginUser(id);
            TempData[TempDataSuccessMessage] = $"社員「{employee.Name}」のログイン情報を削除しました。";
            return RedirectToReturnUrlOrDetails(returnUrl, id);
        }
        catch (Exception e)
        {
            var viewModel = ToLoginUserDeleteViewModel(employee, loginUser);
            viewModel.ErrorMessage = e.Message;
            viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
            viewModel.CancelUrl = NormalizeReturnUrl(cancelUrl) ?? viewModel.ReturnUrl;
            return View("DeleteLoginUser", viewModel);
        }
    }

    /// <summary>
    /// 社員削除確認画面を表示する
    /// </summary>
    [HttpGet]
    public IActionResult Delete(int id, string? returnUrl, string? cancelUrl)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        var employee = _employeeService.GetEmployeeById(id);
        var viewModel = ToDeleteViewModel(employee);
        viewModel.ReturnUrl = NormalizeReturnUrl(returnUrl);
        viewModel.CancelUrl = NormalizeReturnUrl(cancelUrl) ?? viewModel.ReturnUrl;
        return View(viewModel);
    }

    /// <summary>
    /// 社員削除処理を行う
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
            var employee = _employeeService.GetEmployeeById(id);
            _employeeService.Delete(id);
            TempData[TempDataSuccessMessage] = $"社員「{employee.Name}」を削除しました。";
            return RedirectToReturnUrlOrIndex(returnUrl);
        }
        catch (Exception e)
        {
            var employee = _employeeService.GetEmployeeById(id);
            var viewModel = ToDeleteViewModel(employee);
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

    private IActionResult RedirectToReturnUrlOrDetails(string? returnUrl, int employeeId)
    {
        var normalizedReturnUrl = NormalizeReturnUrl(returnUrl);
        return normalizedReturnUrl == null
            ? RedirectToAction(nameof(Details), new { id = employeeId })
            : LocalRedirect(normalizedReturnUrl);
    }

    private void FillFormSupportData(EmployeeFormViewModel form)
    {
        var departments = _departmentService.GetDepartments();
        form.DepartmentOptions = CreateDepartmentOptions(departments);
    }

    private List<SelectListItem> CreateDepartmentOptions(bool includeEmptyOption = false)
    {
        return CreateDepartmentOptions(_departmentService.GetDepartments(), includeEmptyOption);
    }

    private static List<SelectListItem> CreateDepartmentOptions(
        List<DepartmentEntity> departments,
        bool includeEmptyOption = false)
    {
        var options = departments
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
        FillFormSupportData(form);

        if (form.Id <= 0)
        {
            return;
        }

        var employee = _employeeService.GetEmployeeById(form.Id);
        form.CreatedByName = GetUserName(employee.CreatedByUser);
        form.CreatedAtText = FormatDateTime(employee.CreatedAt);
        form.UpdatedByName = GetUserName(employee.UpdatedByUser);
        form.UpdatedAtText = FormatDateTime(employee.UpdatedAt);
        FillLoginUserSummaryData(form, employee);
    }

    private void ClearPasswordFields(EmployeeLoginUserFormViewModel form)
    {
        form.Password = null;
        form.ConfirmPassword = null;
        ClearModelStateValue(nameof(EmployeeLoginUserFormViewModel.Password));
        ClearModelStateValue(nameof(EmployeeLoginUserFormViewModel.ConfirmPassword));
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

    private void NormalizeEditReturnUrls(EmployeeFormViewModel form)
    {
        form.DeleteReturnUrl = NormalizeDeleteReturnUrl(
            form.DeleteReturnUrl,
            form.ReturnUrl,
            Url.Action(nameof(Details), new { id = form.Id }));
        form.ReturnUrl = NormalizeReturnUrl(form.ReturnUrl);
    }

    private void FillLoginUserSummaryData(EmployeeFormViewModel form, EmployeeEntity employee)
    {
        var loginUser = _employeeService.GetLoginUserByEmployeeId(employee.Id);
        form.HasLoginUser = loginUser != null;
        form.CanManageLoginUser = CanManageLoginUser(employee, loginUser);
    }

    private void FillLoginUserFormSupportData(
        EmployeeLoginUserFormViewModel form,
        EmployeeEntity employee,
        bool overwriteLoginId)
    {
        var loginUser = _employeeService.GetLoginUserByEmployeeId(employee.Id);
        FillLoginUserFormSupportData(form, employee, loginUser, overwriteLoginId);
    }

    private static void FillLoginUserFormSupportData(
        EmployeeLoginUserFormViewModel form,
        EmployeeEntity employee,
        LoginUserEntity? loginUser,
        bool overwriteLoginId)
    {
        form.EmployeeId = employee.Id;
        form.EmployeeNo = employee.EmployeeNo;
        form.EmployeeName = employee.Name;
        form.DepartmentName = employee.Department?.Name ?? string.Empty;
        form.HasLoginUser = loginUser != null;

        if (overwriteLoginId || string.IsNullOrWhiteSpace(form.LoginId))
        {
            form.LoginId = loginUser?.LoginId;
        }
    }

    private void ValidatePasswordInput(EmployeeLoginUserFormViewModel form)
    {
        var passwordEntered = !string.IsNullOrWhiteSpace(form.Password);
        var confirmationEntered = !string.IsNullOrWhiteSpace(form.ConfirmPassword);
        if (!form.HasLoginUser && !passwordEntered)
        {
            ModelState.AddModelError(
                nameof(EmployeeLoginUserFormViewModel.Password),
                "ログイン情報を新規登録する場合はパスワードを入力してください。");
        }

        if (confirmationEntered && !passwordEntered)
        {
            ModelState.AddModelError(
                nameof(EmployeeLoginUserFormViewModel.Password),
                "パスワード確認を入力した場合はパスワードも入力してください。");
        }

        if ((passwordEntered || confirmationEntered)
            && string.IsNullOrWhiteSpace(form.ConfirmPassword))
        {
            ModelState.AddModelError(
                nameof(EmployeeLoginUserFormViewModel.ConfirmPassword),
                "パスワード確認を入力してください。");
        }
    }

    private static EmployeeListItemViewModel ToListItemViewModel(EmployeeEntity employee)
    {
        return new EmployeeListItemViewModel
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            DepartmentId = employee.DepartmentId,
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
            DepartmentId = employee.DepartmentId,
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

    private EmployeeLoginUserFormViewModel ToLoginUserFormViewModel(
        EmployeeEntity employee,
        LoginUserEntity? loginUser)
    {
        var viewModel = new EmployeeLoginUserFormViewModel();
        FillLoginUserFormSupportData(viewModel, employee, loginUser, overwriteLoginId: true);
        return viewModel;
    }

    private static EmployeeLoginUserDeleteViewModel ToLoginUserDeleteViewModel(
        EmployeeEntity employee,
        LoginUserEntity loginUser)
    {
        return new EmployeeLoginUserDeleteViewModel
        {
            EmployeeId = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            EmployeeName = employee.Name,
            DepartmentName = employee.Department?.Name ?? string.Empty,
            LoginId = loginUser.LoginId
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

    private static bool IsHumanResourcesEmployee(EmployeeEntity employee)
    {
        return employee.Department?.Name == HumanResourcesDepartmentName;
    }

    private bool CanManageLoginUser(EmployeeEntity employee, LoginUserEntity? loginUser)
    {
        var loginUserId = GetLoginUserId();
        return loginUserId != null
            && CanManageLoginUser(employee, loginUser, loginUserId.Value);
    }

    private static bool CanManageLoginUser(
        EmployeeEntity employee,
        LoginUserEntity? loginUser,
        int currentLoginUserId)
    {
        if (!IsHumanResourcesEmployee(employee))
        {
            return false;
        }

        return loginUser == null || loginUser.Id == currentLoginUserId;
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
