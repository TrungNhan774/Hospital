using BCrypt.Net;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Implements;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(User user)
        {
            var existing = await _repo.GetByUsernameAsync(user.Username);
            if (existing != null)
                return (false, "Username already exists!");

            if (!string.IsNullOrEmpty(user.Email))
            {
                var existingEmail = await _repo.GetByEmailAsync(user.Email);
                if (existingEmail != null)
                    return (false, "Email already exists!");
            }

            // Mã hóa mật khẩu bằng BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.Role = string.IsNullOrEmpty(user.Role) ? "CUSTOMER" : user.Role.ToUpper();
            user.CreatedAt = DateTime.Now;

            await _repo.AddAsync(user);
            return (true, "Registration successful!");
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _repo.GetByUsernameAsync(username);
            if (user == null) return null;

            // Kiểm tra xem mật khẩu có phải là BCrypt hash không
            if (IsBcryptHash(user.Password))
            {
                // Nếu là BCrypt hash
                bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                return valid ? user : null;
            }
            else
            {
                // Nếu là plain text (dữ liệu hiện tại)
                bool valid = password == user.Password;
                if (valid)
                {
                    // Tự động mã hóa mật khẩu bằng BCrypt cho lần đăng nhập tiếp theo
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    await _repo.UpdateAsync(user);
                }
                return valid ? user : null;
            }
        }

        // Phương thức kiểm tra xem chuỗi có phải BCrypt hash không
        private bool IsBcryptHash(string password)
        {
            return password.StartsWith("$2a$") ||
                   password.StartsWith("$2b$") ||
                   password.StartsWith("$2y$");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(string searchString = null)
        {
            return await _repo.GetAllAsync(searchString);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            await _repo.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _repo.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public bool UserExists(int id)
        {
            return _repo.Exists(id);
        }
    }
}
