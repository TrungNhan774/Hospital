using DAL.Models;
using System.Collections.Generic;

namespace DAL.Repositories
{
    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetAll();
        Department? GetById(int id);
        void Add(Department department);
        void Update(Department department);
        void Delete(int id);
    }
}
