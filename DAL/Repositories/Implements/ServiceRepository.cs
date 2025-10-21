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
    public class ServiceRepository : IServiceRepository
    {
        private readonly DbhospitalContext _context;

        public ServiceRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services
                .Where(s => s.IsActive) // chỉ lấy service đang active
                .ToListAsync();
        }

        public async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == id);
        }


        public async Task AddAsync(Service service)
        {
            await _context.Services.AddAsync(service);
        }

        public async Task UpdateAsync(Service service)
        {
            var existing = await _context.Services.FindAsync(service.ServiceId);
            if (existing == null) return;

            existing.Name = service.Name;
            existing.Description = service.Description;
            existing.Price = service.Price;
        }

        public async Task DeleteAsync(int id)
        {
            var service = await GetByIdAsync(id);
            if (service != null)
            {
                service.IsActive = false; // đánh dấu là đã xóa
                _context.Services.Update(service); // cập nhật lại DB
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
