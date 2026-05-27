using EmployeeManagement.Test.Infrastructures;
using EmployeeManagement.Web.Applications.Services.Impls;

namespace EmployeeManagement.Test.Applications.Services;

[TestClass]
[TestCategory("Unit")]
public sealed class LoginAuthorizationServiceTests
{
    private LoginAuthorizationService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new LoginAuthorizationService();
    }

    [TestMethod]
    [DataRow("人事部", true, DisplayName = "人事部所属者はログイン可能")]
    [DataRow("営業部", false, DisplayName = "人事部以外はログイン不可")]
    [DataRow("", false, DisplayName = "部門が取得できない場合はログイン不可")]
    public void CanLogin_ShouldReturnExpectedResult_ByEmployeeDepartment(
        string departmentName,
        bool expected)
    {
        // Arrange
        var loginUser = TestDataFactory.CreateLoginUser(1, "1001", departmentName);
        if (departmentName == string.Empty)
        {
            loginUser.Employee!.Department = null;
        }

        // Act
        var result = _service.CanLogin(loginUser);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
