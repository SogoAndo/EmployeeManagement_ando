namespace EmployeeManagement.Web.Applications.Services;

/// <summary>
/// パスワードのハッシュ化と検証を行うサービスインターフェイス
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// パスワードを保存用のハッシュ文字列に変換する
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// 入力パスワードが保存済みパスワードと一致するか検証する
    /// </summary>
    bool Verify(string password, string storedPassword);
}
