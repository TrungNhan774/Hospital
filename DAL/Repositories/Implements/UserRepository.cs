using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly DbhospitalContext _context;
        public UserRepository(DbhospitalContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                 .Where(u => u.IsActive)
                 .FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        // lay tong nguoi dung
        public async Task<int> CountAllAsync()
        {
            return await _context.Users.CountAsync();
        }
        //lay theo role
        public async Task<int> CountByRoleAsync(string role)
        {
            return await _context.Users.CountAsync(u => u.Role == role);
        }
        //format tim kiem
        private static string NormalizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.ToLower().Trim();

            // Bỏ dấu tiếng Việt
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // 📋 Lấy danh sách người dùng + tìm kiếm + sắp xếp
        public async Task<IEnumerable<User>> GetAllAsync(string search = null, string sortOrder = null)
        {
            var query = _context.Users
                .Where(u => u.IsActive) // 🔹 lọc user còn hoạt động
                .AsQueryable();

            // 🔍 Tìm kiếm (như cũ)
            if (!string.IsNullOrEmpty(search))
            {
                var normalizedSearch = NormalizeText(search);

                query = query
                    .AsEnumerable()
                    .Where(u =>
                        NormalizeText(u.Username).Contains(normalizedSearch) ||
                        NormalizeText(u.FullName).Contains(normalizedSearch) ||
                        NormalizeText(u.Phone ?? "").Contains(normalizedSearch)
                    )
                    .AsQueryable();
            }

            // ↕️ Sắp xếp (như cũ)
            query = sortOrder switch
            {
                "username_asc" => query.OrderBy(u => u.Username),
                "username_desc" => query.OrderByDescending(u => u.Username),
                "fullname_asc" => query.OrderBy(u => u.FullName),
                "fullname_desc" => query.OrderByDescending(u => u.FullName),
                _ => query.OrderBy(u => u.FullName)
            };

            return await Task.FromResult(query.ToList());
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                 .Where(u => u.IsActive)
                 .FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }


        public bool Exists(int id)
        {
            return _context.Users.Any(e => e.UserId == id && e.IsActive);
        }

    }
}
