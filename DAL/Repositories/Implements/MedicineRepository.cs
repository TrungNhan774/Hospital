using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Implements
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly DbhospitalContext _context;

        public MedicineRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Medicine>> GetAllAsync()
        {
            return await _context.Medicines.ToListAsync();
        }

        public async Task<Medicine?> GetByIdAsync(int id)
        {
            return await _context.Medicines.FindAsync(id);
        }

        public async Task AddAsync(Medicine medicine)
        {
            await _context.Medicines.AddAsync(medicine);
        }

        public async Task UpdateAsync(Medicine medicine)
        {
            var existing = await _context.Medicines.FindAsync(medicine.MedicineId);
            if (existing == null) return;

            existing.Name = medicine.Name;
            existing.Description = medicine.Description;
            existing.Unit = medicine.Unit;
            existing.Price = medicine.Price;
            existing.CreatedAt = medicine.CreatedAt;
        }

        public async Task DeleteAsync(int id)
        {
            var medicine = await GetByIdAsync(id);
            if (medicine != null)
                _context.Medicines.Remove(medicine);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}