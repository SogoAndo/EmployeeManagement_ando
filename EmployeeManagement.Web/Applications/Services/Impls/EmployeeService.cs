using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Entities;
using System.Text.RegularExpressions;

namespace EmployeeManagement.Web.Applications.Services.Impls;

/// <summary>
/// 社員情報サービスインターフェイスの実装
/// </summary>
public class EmployeeService : IEmployeeService
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginUserRepository _loginUserRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        ILoginUserRepository loginUserRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _loginUserRepository = loginUserRepository;
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
}
