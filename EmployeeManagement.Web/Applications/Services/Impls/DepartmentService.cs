using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Web.Applications.Services.Impls;

/// <summary>
/// 部門情報サービスインターフェイスの実装
/// </summary>
public class DepartmentService : IDepartmentService
{
    private const string HumanResourcesDepartmentName = "人事部";

    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginUserRepository _loginUserRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DepartmentService(
        IDepartmentRepository departmentRepository,
        ILoginUserRepository loginUserRepository)
    {
        _departmentRepository = departmentRepository;
        _loginUserRepository = loginUserRepository;
    }

    /// <summary>
    /// 部門一覧を取得する
    /// </summary>
    public List<DepartmentEntity> GetDepartments()
    {
        return _departmentRepository.FindAll();
    }

    /// <summary>
    /// 指定されたIdの部門を取得する
    /// </summary>
    public DepartmentEntity GetDepartmentById(int id)
    {
        var department = _departmentRepository.FindById(id);
        if (department == null)
        {
            throw new KeyNotFoundException($"指定された部門ID:{id}は存在しません。");
        }
        return department;
    }

    /// <summary>
    /// 部門を登録する
    /// </summary>
    public void Register(DepartmentEntity department)
    {
        NormalizeForSave(department);
        ValidateForSave(department);

        if (_departmentRepository.ExistsByName(department.Name))
        {
            throw new InvalidOperationException($"部門名:{department.Name}は既に使用されています。");
        }

        department.CreatedAt = DateTime.Now;
        _departmentRepository.Create(department);
    }

    /// <summary>
    /// 部門を更新する
    /// </summary>
    public void Update(DepartmentEntity department)
    {
        var current = GetDepartmentById(department.Id);
        NormalizeForSave(department);
        department.CreatedAt = current.CreatedAt;
        department.CreatedByUserId = current.CreatedByUserId;

        ValidateForSave(department);

        if (current.Name == HumanResourcesDepartmentName
            && department.Name != HumanResourcesDepartmentName)
        {
            throw new InvalidOperationException("人事部はログイン判定に使用しているため、部門名を変更できません。");
        }

        var sameNameDepartment = _departmentRepository.FindByName(department.Name);
        if (sameNameDepartment != null && sameNameDepartment.Id != department.Id)
        {
            throw new InvalidOperationException($"部門名:{department.Name}は既に使用されています。");
        }

        department.UpdatedAt = DateTime.Now;
        _departmentRepository.Update(department);
    }

    /// <summary>
    /// 部門を削除する
    /// </summary>
    public void Delete(int id)
    {
        var department = GetDepartmentById(id);
        if (department.Name == HumanResourcesDepartmentName)
        {
            throw new InvalidOperationException("人事部はログイン判定に使用しているため、削除できません。");
        }

        if (department.Employees.Count > 0)
        {
            throw new InvalidOperationException("所属社員がいる部門は削除できません。");
        }

        _departmentRepository.Delete(department);
    }

    /// <summary>
    /// 部門保存前の簡易チェック
    /// </summary>
    private void ValidateForSave(DepartmentEntity department)
    {
        if (string.IsNullOrWhiteSpace(department.Name))
        {
            throw new ArgumentException("部門名は必須です。");
        }
        if (department.Name.Length > 50)
        {
            throw new ArgumentException("部門名は50文字以内で入力してください。");
        }
        if (_loginUserRepository.FindById(department.CreatedByUserId) == null)
        {
            throw new ArgumentException("指定された登録者は存在しません。");
        }
        if (department.UpdatedByUserId != null
            && _loginUserRepository.FindById(department.UpdatedByUserId.Value) == null)
        {
            throw new ArgumentException("指定された更新者は存在しません。");
        }
    }

    /// <summary>
    /// 保存前に文字列の前後空白を取り除く
    /// </summary>
    private static void NormalizeForSave(DepartmentEntity department)
    {
        department.Name = (department.Name ?? string.Empty).Trim();
    }
}
