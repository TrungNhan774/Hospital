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
    }
}
