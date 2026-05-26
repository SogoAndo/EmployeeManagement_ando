using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Context;
using EmployeeManagement.Web.Infrastructures.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Infrastructures.Repositories;

/// <summary>
/// ログインユーザー情報の操作インターフェイスの実装
/// </summary>
public class LoginUserRepository : ILoginUserRepository
{
    private readonly AppDbContext _appDbContext;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LoginUserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    /// <summary>
    /// 引数Idに一致するログインユーザーを取得する
    /// </summary>
    public LoginUserEntity? FindById(int id)
    {
        return _appDbContext.LoginUsers
            .AsNoTracking()
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Department)
            .FirstOrDefault(u => u.Id == id);
    }

    /// <summary>
    /// 引数ログインIDに一致するログインユーザーを取得する
    /// </summary>
    public LoginUserEntity? FindByLoginId(string loginId)
    {
        return _appDbContext.LoginUsers
            .AsNoTracking()
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Department)
            .FirstOrDefault(u => u.LoginId == loginId);
    }

    /// <summary>
    /// 引数社員IDに一致するログインユーザーを取得する
    /// </summary>
    public LoginUserEntity? FindByEmployeeId(int employeeId)
    {
        return _appDbContext.LoginUsers
            .AsNoTracking()
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Department)
            .FirstOrDefault(u => u.EmployeeId == employeeId);
    }

    /// <summary>
    /// 引数ログインIDの存在有無を取得する
    /// </summary>
    public bool ExistsByLoginId(string loginId)
    {
        return _appDbContext.LoginUsers.Any(u => u.LoginId == loginId);
    }

    /// <summary>
    /// ログインユーザーを登録する
    /// </summary>
    public void Create(LoginUserEntity loginUser)
    {
        _appDbContext.LoginUsers.Add(loginUser);
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// ログインユーザーを更新する
    /// </summary>
    public void Update(LoginUserEntity loginUser)
    {
        _appDbContext.LoginUsers.Update(loginUser);
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// ログインユーザーを削除する
    /// </summary>
    public void Delete(LoginUserEntity loginUser)
    {
        _appDbContext.LoginUsers.Remove(loginUser);
        _appDbContext.SaveChanges();
    }
}
