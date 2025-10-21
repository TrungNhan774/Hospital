using DAL.Models;
using DAL.Repositories;

namespace BLL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentService(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Department> GetAll() => _repo.GetAll();

        public Department? GetById(int id) => _repo.GetById(id);

        public void Add(Department department) => _repo.Add(department);

        public void Update(Department department) => _repo.Update(department);

        public void Delete(int id) => _repo.Delete(id);
    }
}
