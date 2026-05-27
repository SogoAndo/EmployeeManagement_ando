using EmployeeManagement.Web.Applications.Repositories;
using EmployeeManagement.Web.Infrastructures.Entities;

namespace EmployeeManagement.Test.Infrastructures.Fakes;

internal sealed class FakeDepartmentRepository : IDepartmentRepository
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
