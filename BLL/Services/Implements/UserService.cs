using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

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
                return (false, "Tên đăng nhập đã tồn tại!");

            // Mã hóa mật khẩu bằng BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Đặt vai trò mặc định là CUSTOMER nếu không được chọn
            user.Role = string.IsNullOrEmpty(user.Role) ? "CUSTOMER" : user.Role.ToUpper();
            user.CreatedAt = DateTime.Now;

            await _repo.AddAsync(user);
            return (true, "Đăng ký thành công!");
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
    }
}
