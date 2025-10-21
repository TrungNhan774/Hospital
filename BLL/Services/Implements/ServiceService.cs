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
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<Service>> GetAllAsync() => await _serviceRepository.GetAllAsync();
        public async Task<Service> GetByIdAsync(int id) => await _serviceRepository.GetByIdAsync(id);

        public async Task AddAsync(Service service)
        {
            await _serviceRepository.AddAsync(service);
            await _serviceRepository.SaveAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            await _serviceRepository.UpdateAsync(service);
            await _serviceRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _serviceRepository.DeleteAsync(id);
            await _serviceRepository.SaveAsync();
        }
    }
}
