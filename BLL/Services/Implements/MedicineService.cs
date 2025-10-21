using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicineService(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<IEnumerable<Medicine>> GetAllAsync()
        {
            return await _medicineRepository.GetAllAsync();
        }

        public async Task<Medicine?> GetByIdAsync(int id)
        {
            return await _medicineRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Medicine medicine)
        {
            if (medicine.CreatedAt == null)
                medicine.CreatedAt = DateTime.Now;

            await _medicineRepository.AddAsync(medicine);
            await _medicineRepository.SaveAsync();
        }

        public async Task UpdateAsync(Medicine medicine)
        {
            await _medicineRepository.UpdateAsync(medicine);
            await _medicineRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _medicineRepository.DeleteAsync(id);
            await _medicineRepository.SaveAsync();
        }
    }
}