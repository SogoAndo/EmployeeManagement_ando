using EmployeeManagement.Test.Infrastructures;
using EmployeeManagement.Test.Infrastructures.Fakes;
using EmployeeManagement.Web.Applications.Services;
using EmployeeManagement.Web.Applications.Services.Impls;
using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Test.Applications.Services;

[TestClass]
[TestCategory("Integration")]
public sealed class EmployeeServiceLoginUserTests
{
    private static readonly IPasswordHashService PasswordHashService = new IdentityPasswordHashService();

    private FakeEmployeeRepository _employees = null!;
    private FakeDepartmentRepository _departments = null!;
    private FakeLoginUserRepository _loginUsers = null!;
    private EmployeeService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _employees = new FakeEmployeeRepository();
        _departments = new FakeDepartmentRepository();
        _loginUsers = new FakeLoginUserRepository();

        _departments.Departments.Add(TestDataFactory.CreateDepartment(1, "人事部"));
        _departments.Departments.Add(TestDataFactory.CreateDepartment(2, "営業部"));
        _loginUsers.LoginUsers.Add(TestDataFactory.CreateLoginUser(1, "1001", "人事部"));

        _service = new EmployeeService(
            _employees,
            _departments,
            _loginUsers,
            PasswordHashService);
    }

    [TestMethod(DisplayName = "人事部社員にはログイン情報を新規登録できる")]
    public void SaveLoginUser_ShouldCreateLoginUser_WhenEmployeeBelongsToHumanResources()
    {
        // Arrange
        var employee = AddEmployee(2, "1002", "鈴木花子", 1);

        // Act
        _service.SaveLoginUser(employee.Id, "1002", "pass2");

        // Assert
        var loginUser = _loginUsers.FindByEmployeeId(employee.Id);
        Assert.IsNotNull(loginUser);
        Assert.AreEqual("1002", loginUser.LoginId);
        Assert.AreNotEqual("pass2", loginUser.Password);
        Assert.IsTrue(PasswordHashService.Verify("pass2", loginUser.Password));
    }

    [TestMethod(DisplayName = "ログイン情報更新時にパスワード未入力なら現在のパスワードを維持する")]
    public void SaveLoginUser_ShouldKeepCurrentPassword_WhenPasswordIsBlank()
    {
        // Arrange
        var employee = AddEmployee(2, "1002", "鈴木花子", 1);
        _loginUsers.LoginUsers.Add(new LoginUserEntity
        {
            Id = 2,
            LoginId = "1002",
            Password = "current-password",
            EmployeeId = employee.Id,
            Employee = employee
        });

        // Act
        _service.SaveLoginUser(employee.Id, "1003", string.Empty);

        // Assert
        var loginUser = _loginUsers.FindByEmployeeId(employee.Id);
        Assert.IsNotNull(loginUser);
        Assert.AreEqual("1003", loginUser.LoginId);
        Assert.AreEqual("current-password", loginUser.Password);
    }

    [TestMethod(DisplayName = "人事部以外の社員にはログイン情報を設定できない")]
    public void SaveLoginUser_ShouldThrowException_WhenEmployeeDoesNotBelongToHumanResources()
    {
        // Arrange
        var employee = AddEmployee(2, "1002", "鈴木花子", 2);

        // Act
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
            _service.SaveLoginUser(employee.Id, "1002", "pass2"));

        // Assert
        Assert.AreEqual("ログイン情報は人事部の社員にのみ設定できます。", exception.Message);
        Assert.IsNull(_loginUsers.FindByEmployeeId(employee.Id));
    }

    [TestMethod(DisplayName = "ログイン情報未登録の社員は削除時に例外になる")]
    public void DeleteLoginUser_ShouldThrowException_WhenLoginUserDoesNotExist()
    {
        // Arrange
        var employee = AddEmployee(2, "1002", "鈴木花子", 1);

        // Act
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
            _service.DeleteLoginUser(employee.Id));

        // Assert
        Assert.AreEqual("削除対象のログイン情報は登録されていません。", exception.Message);
    }

    private EmployeeEntity AddEmployee(int id, string employeeNo, string name, int departmentId)
    {
        var employee = TestDataFactory.CreateEmployee(
            id,
            employeeNo,
            name,
            $"{employeeNo.ToLowerInvariant()}@example.com",
            departmentId);
        _employees.Employees.Add(employee);
        return employee;
    }
}
