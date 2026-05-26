using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Context;
using EmployeeManagement.Web.Infrastructures.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Infrastructures.Repositories;

/// <summary>
/// 部門情報のCRUD操作インターフェイスの実装
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _appDbContext;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DepartmentRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    /// <summary>
    /// すべての部門を取得する
    /// </summary>
    public List<DepartmentEntity> FindAll()
    {
        return _appDbContext.Departments
            .AsNoTracking()
            .Include(d => d.Employees)
            .Include(d => d.CreatedByUser)
            .ThenInclude(u => u!.Employee)
            .Include(d => d.UpdatedByUser)
            .ThenInclude(u => u!.Employee)
            .OrderBy(d => d.Id)
            .ToList();
    }

    /// <summary>
    /// 引数Idに一致する部門を取得する
    /// </summary>
    public DepartmentEntity? FindById(int id)
    {
        return _appDbContext.Departments
            .AsNoTracking()
            .Include(d => d.Employees)
            .Include(d => d.CreatedByUser)
            .ThenInclude(u => u!.Employee)
            .Include(d => d.UpdatedByUser)
            .ThenInclude(u => u!.Employee)
            .FirstOrDefault(d => d.Id == id);
    }

    /// <summary>
    /// 引数部門名に一致する部門を取得する
    /// </summary>
    public DepartmentEntity? FindByName(string name)
    {
        return _appDbContext.Departments
            .AsNoTracking()
            .FirstOrDefault(d => d.Name == name);
    }

    /// <summary>
    /// 引数部門名の存在有無を取得する
    /// </summary>
    public bool ExistsByName(string name)
    {
        return _appDbContext.Departments.Any(d => d.Name == name);
    }

    /// <summary>
    /// 部門を登録する
    /// </summary>
    public void Create(DepartmentEntity department)
    {
        _appDbContext.Departments.Add(department);
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// 部門を更新する
    /// </summary>
    public void Update(DepartmentEntity department)
    {
        _appDbContext.Departments.Update(department);
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// 部門を削除する
    /// </summary>
    public void Delete(DepartmentEntity department)
    {
        _appDbContext.Departments.Remove(department);
        _appDbContext.SaveChanges();
    }
}
