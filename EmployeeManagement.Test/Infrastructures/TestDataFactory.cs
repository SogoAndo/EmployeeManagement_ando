using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Test.Infrastructures;

internal static class TestDataFactory
{
    public static DepartmentEntity CreateDepartment(int id, string name)
    {
        return new DepartmentEntity
        {
            Id = id,
            Name = name,
            CreatedByUserId = 1,
            CreatedAt = new DateTime(2026, 5, 22, 16, 53, 16)
        };
    }

    public static EmployeeEntity CreateEmployee(
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

    public static LoginUserEntity CreateLoginUser(int id, string loginId, string departmentName)
    {
        var department = CreateDepartment(id, departmentName);
        var employee = CreateEmployee(id, $"{1000 + id}", $"社員{id}", $"employee{id}@example.com", department.Id);
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
}
