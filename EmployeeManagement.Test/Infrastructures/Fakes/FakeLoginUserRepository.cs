using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Test.Infrastructures.Fakes;

internal sealed class FakeLoginUserRepository : ILoginUserRepository
{
    public List<LoginUserEntity> LoginUsers { get; } = new();

    public LoginUserEntity? FindById(int id)
    {
        return LoginUsers.FirstOrDefault(u => u.Id == id);
    }

    public LoginUserEntity? FindByLoginId(string loginId)
    {
        return LoginUsers.FirstOrDefault(u => u.LoginId == loginId);
    }

    public LoginUserEntity? FindByEmployeeId(int employeeId)
    {
        return LoginUsers.FirstOrDefault(u => u.EmployeeId == employeeId);
    }

    public bool ExistsByLoginId(string loginId)
    {
        return LoginUsers.Any(u => u.LoginId == loginId);
    }

    public void Create(LoginUserEntity loginUser)
    {
        if (loginUser.Id == 0)
        {
            loginUser.Id = LoginUsers.Count == 0 ? 1 : LoginUsers.Max(u => u.Id) + 1;
        }

        LoginUsers.Add(loginUser);
    }

    public void Update(LoginUserEntity loginUser)
    {
        Delete(loginUser);
        LoginUsers.Add(loginUser);
    }

    public void UpdatePassword(int id, string password)
    {
        var loginUser = FindById(id)
            ?? throw new KeyNotFoundException($"指定されたログインユーザーID:{id}は存在しません。");

        loginUser.Password = password;
    }

    public void Delete(LoginUserEntity loginUser)
    {
        LoginUsers.RemoveAll(u => u.Id == loginUser.Id);
    }
}
