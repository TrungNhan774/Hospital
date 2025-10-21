using DAL.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    public interface IPatientService
    {
        IEnumerable<Patient> GetAll();
        Patient GetById(int id);
        void Add(Patient patient);
        void Update(Patient patient);
        void Delete(int id);

        IEnumerable<User> GetAllUsers(); // để hiển thị dropdown chọn user
    }
}
