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
using System.Text.Json;
namespace Hospital.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public AccountController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone
            };

            var check = await _userService.RegisterAsync(user);
            if (!check.Success)
            {
                ViewBag.Message = check.Message;
                return View(dto);
            }

            // Tạo OTP + lưu session
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("RegisterEmail", dto.Email);
            HttpContext.Session.SetString("RegisterData", System.Text.Json.JsonSerializer.Serialize(dto));
            HttpContext.Session.SetString("OTP_Expires", DateTime.UtcNow.AddMinutes(5).ToString("O"));

            // Gửi email
            var html = $"<h2>Mã OTP của bạn:</h2><h1 style='color:#007bff'>{otp}</h1><p>Hết hạn sau 5 phút.</p>";
            await _emailService.SendEmailAsync(dto.Email, "Xác thực đăng ký", html);

            return RedirectToAction("VerifyOTP");
        }

        [HttpGet]
        public IActionResult VerifyOTP()
        {
            var email = HttpContext.Session.GetString("RegisterEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Register");
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string otp)
        {
            var savedOtp = HttpContext.Session.GetString("OTP");
            var email = HttpContext.Session.GetString("RegisterEmail");
            var dataJson = HttpContext.Session.GetString("RegisterData");
            var expiresStr = HttpContext.Session.GetString("OTP_Expires");

            if (string.IsNullOrEmpty(savedOtp))
            {
                TempData["Error"] = "Phiên hết hạn!";
                return RedirectToAction("Register");
            }

            if (!DateTime.TryParse(expiresStr, out var expires) || DateTime.UtcNow > expires)
            {
                ClearSession();
                TempData["Error"] = "Mã OTP đã hết hạn!";
                return RedirectToAction("Register");
            }

            if (otp != savedOtp)
            {
                ViewBag.Email = email;
                ViewBag.Error = "Mã OTP sai!";
                return View();
            }

            var dto = System.Text.Json.JsonSerializer.Deserialize<RegisterDTO>(dataJson)!;
            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone
            };

            var result = await _userService.RegisterUserOnlyAsync(user);
            ClearSession();

            if (result.Success)
            {
                TempData["Message"] = "Registration successful";
                TempData["AutoUsername"] = dto.Username;
                return RedirectToAction("Login");
            }

            TempData["Error"] = result.Message;
            return RedirectToAction("Register");
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

        private void ClearSession()
        {
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("RegisterEmail");
            HttpContext.Session.Remove("RegisterData");
            HttpContext.Session.Remove("OTP_Expires");
        }
    }
}
