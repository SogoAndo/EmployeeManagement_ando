using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EmployeeManagement.Web.Applications.Services.Impls;

/// <summary>
/// 社員情報サービスインターフェイスの実装
/// </summary>
public class EmployeeService : IEmployeeService
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    private const string LoginIdPattern = @"^\d{4}$";
    private const string HumanResourcesDepartmentName = "人事部";

    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginUserRepository _loginUserRepository;
    private readonly IPasswordHashService _passwordHashService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        ILoginUserRepository loginUserRepository,
        IPasswordHashService passwordHashService)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _loginUserRepository = loginUserRepository;
        _passwordHashService = passwordHashService;
    }

    /// <summary>
    /// 社員一覧を取得する
    /// </summary>
    public List<EmployeeEntity> GetEmployees()
    {
        return _employeeRepository.FindAll();
    }

    /// <summary>
    /// 指定されたIdの社員を取得する
    /// </summary>
    public EmployeeEntity GetEmployeeById(int id)
    {
        var employee = _employeeRepository.FindById(id);
        if (employee == null)
        {
            throw new KeyNotFoundException($"指定された社員ID:{id}は存在しません。");
        }
        return employee;
    }

    /// <summary>
    /// 社員を登録する
    /// </summary>
    public void Register(EmployeeEntity employee)
    {
        NormalizeForSave(employee);
        ValidateForSave(employee);

        if (_employeeRepository.ExistsByEmployeeNo(employee.EmployeeNo))
        {
            throw new InvalidOperationException($"社員番号:{employee.EmployeeNo}は既に使用されています。");
        }

        employee.CreatedAt = DateTime.Now;
        _employeeRepository.Create(employee);
    }

    /// <summary>
    /// 社員とログイン情報を登録する
    /// </summary>
    public void Register(EmployeeEntity employee, string? loginId, string? password)
    {
        NormalizeForSave(employee);
        ValidateForSave(employee);
        var loginUser = BuildLoginUserForCreate(employee, loginId, password);

        if (_employeeRepository.ExistsByEmployeeNo(employee.EmployeeNo))
        {
            throw new InvalidOperationException($"社員番号:{employee.EmployeeNo}は既に使用されています。");
        }

        employee.CreatedAt = DateTime.Now;
        _employeeRepository.Create(employee);

        if (loginUser != null)
        {
            loginUser.EmployeeId = employee.Id;
            _loginUserRepository.Create(loginUser);
        }
    }

    /// <summary>
    /// 社員を更新する
    /// </summary>
    public void Update(EmployeeEntity employee)
    {
        var current = GetEmployeeById(employee.Id);
        employee.CreatedAt = current.CreatedAt;
        employee.CreatedByUserId = current.CreatedByUserId;

        NormalizeForSave(employee);
        ValidateForSave(employee);

        var sameEmployeeNo = _employeeRepository.FindByEmployeeNo(employee.EmployeeNo);
        if (sameEmployeeNo != null && sameEmployeeNo.Id != employee.Id)
        {
            throw new InvalidOperationException($"社員番号:{employee.EmployeeNo}は既に使用されています。");
        }

        employee.UpdatedAt = DateTime.Now;
        _employeeRepository.Update(employee);
    }

    /// <summary>
    /// 社員とログイン情報を更新する
    /// </summary>
    public void Update(EmployeeEntity employee, string? loginId, string? password)
    {
        var current = GetEmployeeById(employee.Id);
        employee.CreatedAt = current.CreatedAt;
        employee.CreatedByUserId = current.CreatedByUserId;

        NormalizeForSave(employee);
        ValidateForSave(employee);

        var sameEmployeeNo = _employeeRepository.FindByEmployeeNo(employee.EmployeeNo);
        if (sameEmployeeNo != null && sameEmployeeNo.Id != employee.Id)
        {
            throw new InvalidOperationException($"社員番号:{employee.EmployeeNo}は既に使用されています。");
        }

        var loginUser = BuildLoginUserForUpdate(employee, loginId, password);
        employee.UpdatedAt = DateTime.Now;
        _employeeRepository.Update(employee);

        if (loginUser == null)
        {
            return;
        }

        if (loginUser.Id == 0)
        {
            _loginUserRepository.Create(loginUser);
            return;
        }

        _loginUserRepository.Update(loginUser);
    }

    /// <summary>
    /// 社員のログイン情報を登録または更新する
    /// </summary>
    public void SaveLoginUser(int employeeId, string? loginId, string? password)
    {
        var employee = GetEmployeeById(employeeId);
        var department = GetDepartment(employee.DepartmentId);
        if (!IsHumanResourcesDepartment(department))
        {
            throw new InvalidOperationException("ログイン情報は人事部の社員にのみ設定できます。");
        }

        loginId = NormalizeLoginId(loginId);
        ValidateLoginId(loginId);

        var currentLoginUser = _loginUserRepository.FindByEmployeeId(employee.Id);
        if (currentLoginUser == null)
        {
            ValidateNewPassword(password);
            EnsureLoginIdCanBeUsed(loginId!, employee.Id);
            _loginUserRepository.Create(new LoginUserEntity
            {
                EmployeeId = employee.Id,
                LoginId = loginId!,
                Password = _passwordHashService.Hash(password!)
            });
            return;
        }

        ValidateOptionalPassword(password);
        EnsureLoginIdCanBeUsed(loginId!, employee.Id);

        currentLoginUser.LoginId = loginId!;
        if (!string.IsNullOrWhiteSpace(password))
        {
            currentLoginUser.Password = _passwordHashService.Hash(password!);
        }

        _loginUserRepository.Update(currentLoginUser);
    }

    /// <summary>
    /// 指定された社員IDに紐づくログインユーザーを取得する
    /// </summary>
    public LoginUserEntity? GetLoginUserByEmployeeId(int employeeId)
    {
        return _loginUserRepository.FindByEmployeeId(employeeId);
    }

    /// <summary>
    /// ログインIDが指定された社員で利用可能か判定する
    /// </summary>
    public bool IsLoginIdAvailable(string? loginId, int employeeId)
    {
        loginId = NormalizeLoginId(loginId);
        if (loginId == null)
        {
            return true;
        }

        var sameLoginUser = _loginUserRepository.FindByLoginId(loginId);
        return sameLoginUser == null || sameLoginUser.EmployeeId == employeeId;
    }

    /// <summary>
    /// 社員のログイン情報を削除する
    /// </summary>
    public void DeleteLoginUser(int employeeId)
    {
        var employee = GetEmployeeById(employeeId);
        var loginUser = _loginUserRepository.FindByEmployeeId(employee.Id);
        if (loginUser == null)
        {
            throw new InvalidOperationException("削除対象のログイン情報は登録されていません。");
        }

        try
        {
            _loginUserRepository.Delete(loginUser);
        }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("登録者または更新者として利用されているため、ログイン情報を削除できません。", e);
        }
    }

    /// <summary>
    /// 社員を削除する
    /// </summary>
    public void Delete(int id)
    {
        var employee = GetEmployeeById(id);
        if (_loginUserRepository.FindByEmployeeId(employee.Id) != null)
        {
            throw new InvalidOperationException("ログインユーザーに紐づく社員は削除できません。");
        }

        _employeeRepository.Delete(employee);
    }

    /// <summary>
    /// 社員保存前の簡易チェック
    /// </summary>
    private void ValidateForSave(EmployeeEntity employee)
    {
        if (string.IsNullOrWhiteSpace(employee.EmployeeNo))
        {
            throw new ArgumentException("社員番号は必須です。");
        }
        if (employee.EmployeeNo.Length > 10)
        {
            throw new ArgumentException("社員番号は10文字以内で入力してください。");
        }
        if (!Regex.IsMatch(employee.EmployeeNo, "^[A-Za-z0-9]+$"))
        {
            throw new ArgumentException("社員番号は半角英数字で入力してください。");
        }
        if (string.IsNullOrWhiteSpace(employee.Name))
        {
            throw new ArgumentException("社員名は必須です。");
        }
        if (employee.Name.Length > 50)
        {
            throw new ArgumentException("社員名は50文字以内で入力してください。");
        }
        if (!string.IsNullOrWhiteSpace(employee.Email) && employee.Email.Length > 100)
        {
            throw new ArgumentException("メールアドレスは100文字以内で入力してください。");
        }
        if (!string.IsNullOrWhiteSpace(employee.Email)
            && !Regex.IsMatch(employee.Email, EmailPattern))
        {
            throw new ArgumentException("メールアドレスはexample@example.comの形式で入力してください。");
        }
        if (employee.HireDate == default)
        {
            throw new ArgumentException("入社日は必須です。");
        }
        if (_departmentRepository.FindById(employee.DepartmentId) == null)
        {
            throw new ArgumentException("指定された部門は存在しません。");
        }
        if (_loginUserRepository.FindById(employee.CreatedByUserId) == null)
        {
            throw new ArgumentException("指定された登録者は存在しません。");
        }
        if (employee.UpdatedByUserId != null
            && _loginUserRepository.FindById(employee.UpdatedByUserId.Value) == null)
        {
            throw new ArgumentException("指定された更新者は存在しません。");
        }
    }

    /// <summary>
    /// 保存前に文字列の前後空白を取り除く
    /// </summary>
    private static void NormalizeForSave(EmployeeEntity employee)
    {
        employee.EmployeeNo = (employee.EmployeeNo ?? string.Empty).Trim();
        employee.Name = (employee.Name ?? string.Empty).Trim();
        employee.Email = string.IsNullOrWhiteSpace(employee.Email)
            ? null
            : employee.Email.Trim();
    }

    private LoginUserEntity? BuildLoginUserForCreate(
        EmployeeEntity employee,
        string? loginId,
        string? password)
    {
        loginId = NormalizeLoginId(loginId);
        var department = GetDepartment(employee.DepartmentId);
        if (!IsHumanResourcesDepartment(department))
        {
            ValidateNoLoginInput(loginId, password);
            return null;
        }

        ValidateLoginId(loginId);
        ValidateNewPassword(password);
        EnsureLoginIdCanBeUsed(loginId!, employee.Id);

        return new LoginUserEntity
        {
            LoginId = loginId!,
            Password = _passwordHashService.Hash(password!)
        };
    }

    private LoginUserEntity? BuildLoginUserForUpdate(
        EmployeeEntity employee,
        string? loginId,
        string? password)
    {
        loginId = NormalizeLoginId(loginId);
        var department = GetDepartment(employee.DepartmentId);
        var currentLoginUser = _loginUserRepository.FindByEmployeeId(employee.Id);

        if (!IsHumanResourcesDepartment(department))
        {
            ValidateNoLoginInput(loginId, password);
            return null;
        }

        ValidateLoginId(loginId);
        if (currentLoginUser == null)
        {
            ValidateNewPassword(password);
            EnsureLoginIdCanBeUsed(loginId!, employee.Id);

            return new LoginUserEntity
            {
                EmployeeId = employee.Id,
                LoginId = loginId!,
                Password = _passwordHashService.Hash(password!)
            };
        }

        ValidateOptionalPassword(password);
        EnsureLoginIdCanBeUsed(loginId!, employee.Id);

        currentLoginUser.LoginId = loginId!;
        if (!string.IsNullOrWhiteSpace(password))
        {
            currentLoginUser.Password = _passwordHashService.Hash(password!);
        }

        return currentLoginUser;
    }

    private DepartmentEntity GetDepartment(int departmentId)
    {
        var department = _departmentRepository.FindById(departmentId);
        if (department == null)
        {
            throw new ArgumentException("指定された部門は存在しません。");
        }

        return department;
    }

    private static bool IsHumanResourcesDepartment(DepartmentEntity department)
    {
        return department.Name == HumanResourcesDepartmentName;
    }

    private static string? NormalizeLoginId(string? loginId)
    {
        return string.IsNullOrWhiteSpace(loginId) ? null : loginId.Trim();
    }

    private static void ValidateNoLoginInput(string? loginId, string? password)
    {
        if (!string.IsNullOrWhiteSpace(loginId) || !string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("ログイン情報は人事部の社員にのみ設定できます。");
        }
    }

    private static void ValidateLoginId(string? loginId)
    {
        if (string.IsNullOrWhiteSpace(loginId))
        {
            throw new ArgumentException("人事部の社員にはログインIDを入力してください。");
        }
        if (!Regex.IsMatch(loginId, LoginIdPattern))
        {
            throw new ArgumentException("ログインIDは4桁の数字で入力してください。");
        }
    }

    private static void ValidateNewPassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("人事部の社員にはパスワードを入力してください。");
        }

        ValidateOptionalPassword(password);
    }

    private static void ValidateOptionalPassword(string? password)
    {
        if (password?.Length > 255)
        {
            throw new ArgumentException("パスワードは255文字以内で入力してください。");
        }
    }

    private void EnsureLoginIdCanBeUsed(string loginId, int employeeId)
    {
        var sameLoginUser = _loginUserRepository.FindByLoginId(loginId);
        if (sameLoginUser != null && sameLoginUser.EmployeeId != employeeId)
        {
            throw new InvalidOperationException($"ログインID:{loginId}は既に使用されています。");
        }
    }
}
