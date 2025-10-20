using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user); // Thêm method này
        Task<int> CountAllAsync();
        Task<int> CountByRoleAsync(string role);
        Task<List<User>> GetAllAsync(string search = null, string sortOrder = null);
    }
}
