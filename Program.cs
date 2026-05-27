using Microsoft.AspNetCore.Identity;

Console.Write("パスワード入力: ");
string? password = Console.ReadLine();

if (string.IsNullOrEmpty(password))
{
    Console.WriteLine("パスワード未入力");
    return;
}

var passwordHasher = new PasswordHasher<string>();

string hashedPassword =
    passwordHasher.HashPassword(null, password);

Console.WriteLine();
Console.WriteLine("=== ハッシュ値 ===");
Console.WriteLine(hashedPassword);
