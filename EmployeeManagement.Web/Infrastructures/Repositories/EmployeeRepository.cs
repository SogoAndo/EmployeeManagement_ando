using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Context;
using EmployeeManagement.Web.Infrastructures.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Infrastructures.Repositories;

/// <summary>
/// 社員情報のCRUD操作インターフェイスの実装
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _appDbContext;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EmployeeRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    /// <summary>
    /// すべての社員を取得する
    /// </summary>
    public List<EmployeeEntity> FindAll()
    {
        return _appDbContext.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.CreatedByUser)
            .ThenInclude(u => u!.Employee)
            .Include(e => e.UpdatedByUser)
            .ThenInclude(u => u!.Employee)
            .OrderBy(e => e.EmployeeNo)
            .ToList();
    }

    /// <summary>
    /// 引数Idに一致する社員を取得する
    /// </summary>
    public EmployeeEntity? FindById(int id)
    {
        return _appDbContext.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.CreatedByUser)
            .ThenInclude(u => u!.Employee)
            .Include(e => e.UpdatedByUser)
            .ThenInclude(u => u!.Employee)
            .FirstOrDefault(e => e.Id == id);
    }

    /// <summary>
    /// 引数社員番号に一致する社員を取得する
    /// </summary>
    public EmployeeEntity? FindByEmployeeNo(string employeeNo)
    {
        return _appDbContext.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefault(e => e.EmployeeNo == employeeNo);
    }

    /// <summary>
    /// 引数社員番号の存在有無を取得する
    /// </summary>
    public bool ExistsByEmployeeNo(string employeeNo)
    {
        return _appDbContext.Employees.Any(e => e.EmployeeNo == employeeNo);
    }

    /// <summary>
    /// 社員を登録する
    /// </summary>
    public void Create(EmployeeEntity employee)
    {
        _appDbContext.Employees.Add(employee);
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// 社員を更新する
    /// </summary>
    public void Update(EmployeeEntity employee)
    {
        _appDbContext.Employees.Update(employee);
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// 社員を削除する
    /// </summary>
    public void Delete(EmployeeEntity employee)
    {
        _appDbContext.Employees.Remove(employee);
        _appDbContext.SaveChanges();
    }
}
