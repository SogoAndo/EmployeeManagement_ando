using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Web.Applications.Services.Impls;

/// <summary>
/// ASP.NET Core IdentityのPasswordHasherによるパスワードハッシュサービス
/// </summary>
public class IdentityPasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    /// <summary>
    /// パスワードを保存用のハッシュ文字列に変換する
    /// </summary>
    public string Hash(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    /// <summary>
    /// 入力パスワードが保存済みパスワードと一致するか検証する
    /// </summary>
    public bool Verify(string password, string storedPassword)
    {
        try
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, storedPassword, password);
            return result != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
