using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DbhospitalContext _context;

        public PatientRepository()
        {
            _context = new DbhospitalContext();
        }

        public IEnumerable<Patient> GetAll()
        {
            return _context.Patients
                .Include(p => p.User)
                .ToList();
        }

        public Patient GetById(int id)
        {
            return _context.Patients
                .Include(p => p.User)
                .FirstOrDefault(p => p.PatientId == id);
        }

        public void Add(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void Update(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                _context.SaveChanges();
            }
        }
        public async Task<Patient?> GetByIdDAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.MedicalRecords)
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }
    }
}
