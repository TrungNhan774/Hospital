using BLL.Services;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hospital.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            // Kiểm tra validation của RegisterDTO
            if (!ModelState.IsValid)
            {
                return View(registerDTO); // Trả về view với lỗi validation
            }

            // Ánh xạ từ RegisterDTO sang User
            var user = new User
            {
                Username = registerDTO.Username,
                Password = registerDTO.Password,
                FullName = registerDTO.FullName,
                Email = registerDTO.Email,
                Phone = registerDTO.Phone,
                Role = "CUSTOMER"
            };

            var result = await _userService.RegisterAsync(user);
            ViewBag.Message = result.Message;

            if (result.Success)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("Login");
            }

            return View(registerDTO);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.LoginAsync(username, password);
            if (user == null)
            {
                ViewBag.Message = "Wrong username or password!";
                return View();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("FullName", user.FullName ?? "")
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Chuyển hướng theo vai trò
            string redirectUrl = user.Role switch
            {
                "ADMIN" => Url.Action("Index", "Admin"),
                "DOCTOR" => "https://localhost:7210/Doctors/Dashboard",
                "CUSTOMER" => Url.Action("Index", "Home")
            };

            return Redirect(redirectUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);  
            return RedirectToAction("Index", "Home");
        }
    }
}
