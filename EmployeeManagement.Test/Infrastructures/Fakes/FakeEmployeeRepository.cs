using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Test.Infrastructures.Fakes;

internal sealed class FakeEmployeeRepository : IEmployeeRepository
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
