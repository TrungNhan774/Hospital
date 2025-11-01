using BLL.Services.Implements;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string Message)> RegisterAsync(User user);
        Task<User?> LoginAsync(string username, string password);
        Task<IEnumerable<User>> GetAllUsersAsync(string searchString = null);
        Task<User> GetUserByIdAsync(int id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        bool UserExists(int id);
        Task<(bool Success, string Message)> RegisterUserOnlyAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
