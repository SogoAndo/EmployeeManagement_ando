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
        var current = _appDbContext.LoginUsers.FirstOrDefault(u => u.Id == loginUser.Id)
            ?? throw new KeyNotFoundException($"指定されたログインユーザーID:{loginUser.Id}は存在しません。");

        current.LoginId = loginUser.LoginId;
        current.Password = loginUser.Password;
        current.EmployeeId = loginUser.EmployeeId;
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// ログインユーザーのパスワードを更新する
    /// </summary>
    public void UpdatePassword(int id, string password)
    {
        var current = _appDbContext.LoginUsers.FirstOrDefault(u => u.Id == id)
            ?? throw new KeyNotFoundException($"指定されたログインユーザーID:{id}は存在しません。");

        current.Password = password;
        _appDbContext.SaveChanges();
    }

    /// <summary>
    /// ログインユーザーを削除する
    /// </summary>
    public void Delete(LoginUserEntity loginUser)
    {
        var current = _appDbContext.LoginUsers.FirstOrDefault(u => u.Id == loginUser.Id);
        if (current == null)
        {
            return;
        }

        _appDbContext.LoginUsers.Remove(current);
        _appDbContext.SaveChanges();
    }
}
