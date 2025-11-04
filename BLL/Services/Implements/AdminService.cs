using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.ViewModels;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        public AdminService(IUserRepository userRepository, IAdminRepository adminRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepository.CountAllAsync();
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            return await _userRepository.CountByRoleAsync("DOCTOR");
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            return await _userRepository.CountByRoleAsync("CUSTOMER");
        }
        public async Task<List<User>> GetAllAsync(string search = null, string sortOrder = null)
        {
            return (List<User>)await _userRepository.GetAllAsync(search, sortOrder);
        }
        public async Task<List<TopDoctorViewModel>> GetTopDoctorsByAppointmentsAsync(int top = 5)
        {
            return await _adminRepository.GetTopDoctorsByAppointmentsAsync(top);
        }

        public async Task<List<MonthlyRevenueViewModel>> GetMonthlyRevenueAsync()
        {
            return await _adminRepository.GetMonthlyRevenueAsync();
        }

        public async Task<List<PatientCountViewModel>> GetPatientCountAsync(string type = "month")
        {
            return await _adminRepository.GetPatientCountAsync(type);
        }

    }
}
