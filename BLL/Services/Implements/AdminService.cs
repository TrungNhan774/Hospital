using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;

        public AdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            return await _userRepository.GetAllAsync(search, sortOrder);
        }
    }
}
