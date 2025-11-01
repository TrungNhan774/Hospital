using DAL.Models;
using System.Collections.Generic;

namespace DAL.Repositories
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAll();
        Patient GetById(int id);
        void Add(Patient patient);
        void Update(Patient patient);
        void Delete(int id);
        Task<Patient?> GetByIdAsync(int id);
        Task UpdateAsync(Patient patient);
    }
}
