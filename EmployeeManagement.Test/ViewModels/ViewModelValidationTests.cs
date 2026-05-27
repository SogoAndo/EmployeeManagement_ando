using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Web.ViewModels;

namespace EmployeeManagement.Test.ViewModels;

[TestClass]
[TestCategory("Unit")]
public sealed class ViewModelValidationTests
{
    [TestMethod]
    [DataRow("", "pass1", "ログインIDを入力してください。", DisplayName = "ログインID未入力")]
    [DataRow("abc1", "pass1", "ログインIDは4桁の数字で入力してください。", DisplayName = "ログインID形式不正")]
    [DataRow("1001", "", "パスワードを入力してください。", DisplayName = "パスワード未入力")]
    public void LoginViewModel_ShouldReturnValidationError_WhenInputIsInvalid(
        string loginId,
        string password,
        string expectedMessage)
    {
        // Arrange
        var form = new LoginViewModel
        {
            LoginId = loginId,
            Password = password
        };

        // Act
        var validationResults = Validate(form);

        // Assert
        Assert.IsTrue(validationResults.Any(r => r.ErrorMessage == expectedMessage));
    }

    [TestMethod(DisplayName = "パスワード確認が一致しない場合はエラーになる")]
    public void EmployeeLoginUserFormViewModel_ShouldReturnValidationError_WhenConfirmPasswordDoesNotMatch()
    {
        // Arrange
        var form = new EmployeeLoginUserFormViewModel
        {
            EmployeeId = 1,
            LoginId = "1001",
            Password = "pass1",
            ConfirmPassword = "pass2"
        };

        // Act
        var validationResults = Validate(form);

        // Assert
        Assert.IsTrue(validationResults.Any(r =>
            r.ErrorMessage == "パスワードとパスワード確認が一致しません。"));
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);
        return results;
    }
}
