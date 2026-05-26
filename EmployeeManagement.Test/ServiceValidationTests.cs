using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Applications.Services.Impls;
using EmployeeManagement.Web.Infrastructures.Entities;
using EmployeeManagement.Web.ViewModels;

namespace EmployeeManagement.Test;

[TestClass]
public sealed class ServiceValidationTests
{
    [TestMethod]
    public void LoginAuthorizationService_CanLogin_ReturnsTrue_WhenEmployeeBelongsToHumanResources()
    {
        var service = new LoginAuthorizationService();
        var loginUser = CreateLoginUser(1, "1001", "人事部");

        var result = service.CanLogin(loginUser);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void LoginAuthorizationService_CanLogin_ReturnsFalse_WhenEmployeeDoesNotBelongToHumanResources()
    {
        var service = new LoginAuthorizationService();
        var loginUser = CreateLoginUser(3, "2001", "営業部");

        var result = service.CanLogin(loginUser);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DepartmentService_Update_Throws_WhenRenamingHumanResourcesDepartment()
    {
        var departments = new FakeDepartmentRepository();
        departments.Departments.Add(CreateDepartment(1, "人事部"));
        var loginUsers = new FakeLoginUserRepository();
        loginUsers.LoginUsers.Add(CreateLoginUser(1, "1001", "人事部"));
        var service = new DepartmentService(departments, loginUsers);

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
            service.Update(new DepartmentEntity
            {
                Id = 1,
                Name = "経営管理部",
                UpdatedByUserId = 1
            }));

        Assert.AreEqual("人事部はログイン判定に使用しているため、部門名を変更できません。", exception.Message);
    }

    [TestMethod]
    public void DepartmentService_Delete_Throws_WhenDeletingHumanResourcesDepartment()
    {
        var departments = new FakeDepartmentRepository();
        departments.Departments.Add(CreateDepartment(1, "人事部"));
        var loginUsers = new FakeLoginUserRepository();
        var service = new DepartmentService(departments, loginUsers);

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => service.Delete(1));

        Assert.AreEqual("人事部はログイン判定に使用しているため、削除できません。", exception.Message);
    }

    [TestMethod]
    public void DepartmentService_Delete_Throws_WhenDepartmentHasEmployees()
    {
        var departments = new FakeDepartmentRepository();
        var department = CreateDepartment(2, "営業部");
        department.Employees.Add(CreateEmployee(2, "E0002", "佐藤健", "sato@example.com", department.Id));
        departments.Departments.Add(department);
        var loginUsers = new FakeLoginUserRepository();
        var service = new DepartmentService(departments, loginUsers);

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => service.Delete(2));

        Assert.AreEqual("所属社員がいる部門は削除できません。", exception.Message);
    }

    [TestMethod]
    public void EmployeeService_Register_Throws_WhenEmailFormatIsInvalid()
    {
        var employees = new FakeEmployeeRepository();
        var departments = new FakeDepartmentRepository();
        departments.Departments.Add(CreateDepartment(1, "人事部"));
        var loginUsers = new FakeLoginUserRepository();
        loginUsers.LoginUsers.Add(CreateLoginUser(1, "1001", "人事部"));
        var service = new EmployeeService(employees, departments, loginUsers);

        var exception = Assert.ThrowsExactly<ArgumentException>(() =>
            service.Register(new EmployeeEntity
            {
                EmployeeNo = "E9999",
                Name = "確認太郎",
                Email = "mail",
                HireDate = new DateTime(2026, 5, 25),
                DepartmentId = 1,
                CreatedByUserId = 1
            }));

        Assert.AreEqual("メールアドレスはexample@example.comの形式で入力してください。", exception.Message);
        Assert.IsEmpty(employees.Employees);
    }

    [TestMethod]
    public void EmployeeService_Register_CreatesEmployee_WhenEmailIsBlank()
    {
        var employees = new FakeEmployeeRepository();
        var departments = new FakeDepartmentRepository();
        departments.Departments.Add(CreateDepartment(1, "人事部"));
        var loginUsers = new FakeLoginUserRepository();
        loginUsers.LoginUsers.Add(CreateLoginUser(1, "1001", "人事部"));
        var service = new EmployeeService(employees, departments, loginUsers);

        service.Register(new EmployeeEntity
        {
            EmployeeNo = " E9999 ",
            Name = " 確認太郎 ",
            Email = "   ",
            HireDate = new DateTime(2026, 5, 25),
            DepartmentId = 1,
            CreatedByUserId = 1
        });

        Assert.HasCount(1, employees.Employees);
        Assert.AreEqual("E9999", employees.Employees[0].EmployeeNo);
        Assert.AreEqual("確認太郎", employees.Employees[0].Name);
        Assert.IsNull(employees.Employees[0].Email);
    }

    [TestMethod]
    public void EmployeeFormViewModel_ValidationFails_WhenEmailFormatIsInvalid()
    {
        var form = new EmployeeFormViewModel
        {
            EmployeeNo = "E9999",
            Name = "確認太郎",
            Email = "mail",
            HireDate = new DateTime(2026, 5, 25),
            DepartmentId = 1
        };

        var validationResults = Validate(form);

        Assert.IsTrue(validationResults.Any(r =>
            r.ErrorMessage == "メールアドレスはexample@example.comの形式で入力してください。"));
    }

    [TestMethod]
    public void LoginViewModel_ValidationFails_WhenLoginIdIsNotFourDigits()
    {
        var form = new LoginViewModel
        {
            LoginId = "abcd",
            Password = "pass1"
        };

        var validationResults = Validate(form);

        Assert.IsTrue(validationResults.Any(r =>
            r.ErrorMessage == "ログインIDは4桁の数字で入力してください。"));
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

    private static DepartmentEntity CreateDepartment(int id, string name)
    {
        return new DepartmentEntity
        {
            Id = id,
            Name = name,
            CreatedByUserId = 1,
            CreatedAt = new DateTime(2026, 5, 22, 16, 53, 16)
        };
    }

    private static EmployeeEntity CreateEmployee(
        int id,
        string employeeNo,
        string name,
        string email,
        int departmentId)
    {
        return new EmployeeEntity
        {
            Id = id,
            EmployeeNo = employeeNo,
            Name = name,
            Email = email,
            HireDate = new DateTime(2026, 5, 25),
            DepartmentId = departmentId,
            CreatedByUserId = 1,
            CreatedAt = new DateTime(2026, 5, 22, 16, 53, 16)
        };
    }

    private static LoginUserEntity CreateLoginUser(int id, string loginId, string departmentName)
    {
        var department = CreateDepartment(id, departmentName);
        var employee = CreateEmployee(id, $"E{id:0000}", $"社員{id}", $"employee{id}@example.com", department.Id);
        employee.Department = department;

        return new LoginUserEntity
        {
            Id = id,
            LoginId = loginId,
            Password = $"pass{id}",
            EmployeeId = employee.Id,
            Employee = employee
        };
    }

    private sealed class FakeEmployeeRepository : IEmployeeRepository
    {
        public List<EmployeeEntity> Employees { get; } = new();

        public List<EmployeeEntity> FindAll()
        {
            return Employees.ToList();
        }

        public EmployeeEntity? FindById(int id)
        {
            return Employees.FirstOrDefault(e => e.Id == id);
        }

        public EmployeeEntity? FindByEmployeeNo(string employeeNo)
        {
            return Employees.FirstOrDefault(e => e.EmployeeNo == employeeNo);
        }

        public bool ExistsByEmployeeNo(string employeeNo)
        {
            return Employees.Any(e => e.EmployeeNo == employeeNo);
        }

        public void Create(EmployeeEntity employee)
        {
            if (employee.Id == 0)
            {
                employee.Id = Employees.Count == 0 ? 1 : Employees.Max(e => e.Id) + 1;
            }

            Employees.Add(employee);
        }

        public void Update(EmployeeEntity employee)
        {
            Delete(employee);
            Employees.Add(employee);
        }

        public void Delete(EmployeeEntity employee)
        {
            Employees.RemoveAll(e => e.Id == employee.Id);
        }
    }

    private sealed class FakeDepartmentRepository : IDepartmentRepository
    {
        public List<DepartmentEntity> Departments { get; } = new();

        public List<DepartmentEntity> FindAll()
        {
            return Departments.ToList();
        }

        public DepartmentEntity? FindById(int id)
        {
            return Departments.FirstOrDefault(d => d.Id == id);
        }

        public DepartmentEntity? FindByName(string name)
        {
            return Departments.FirstOrDefault(d => d.Name == name);
        }

        public bool ExistsByName(string name)
        {
            return Departments.Any(d => d.Name == name);
        }

        public void Create(DepartmentEntity department)
        {
            if (department.Id == 0)
            {
                department.Id = Departments.Count == 0 ? 1 : Departments.Max(d => d.Id) + 1;
            }

            Departments.Add(department);
        }

        public void Update(DepartmentEntity department)
        {
            Delete(department);
            Departments.Add(department);
        }

        public void Delete(DepartmentEntity department)
        {
            Departments.RemoveAll(d => d.Id == department.Id);
        }
    }

    private sealed class FakeLoginUserRepository : ILoginUserRepository
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
            LoginUsers.Add(loginUser);
        }

        public void Update(LoginUserEntity loginUser)
        {
            Delete(loginUser);
            LoginUsers.Add(loginUser);
        }

        public void Delete(LoginUserEntity loginUser)
        {
            LoginUsers.RemoveAll(u => u.Id == loginUser.Id);
        }
    }
}
