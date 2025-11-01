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

            // Generate OTP + save to session
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("RegisterEmail", dto.Email);
            HttpContext.Session.SetString("RegisterData", JsonSerializer.Serialize(dto));
            HttpContext.Session.SetString("OTP_Expires", DateTime.UtcNow.AddMinutes(5).ToString("O"));

            // Send email
            var html = $"<h2>Your OTP code:</h2><h1 style='color:#007bff'>{otp}</h1><p>Expires in 5 minutes.</p>";
            await _emailService.SendEmailAsync(dto.Email, "Account Verification", html);

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
                TempData["Error"] = "Session expired!";
                return RedirectToAction("Register");
            }

            if (!DateTime.TryParse(expiresStr, out var expires) || DateTime.UtcNow > expires)
            {
                ClearSession();
                TempData["Error"] = "OTP has expired!";
                return RedirectToAction("Register");
            }

            if (otp != savedOtp)
            {
                ViewBag.Email = email;
                ViewBag.Error = "Invalid OTP!";
                return View();
            }

            var dto = JsonSerializer.Deserialize<RegisterDTO>(dataJson)!;
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
                ViewBag.Message = "Invalid username or password!";
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

            // Redirect based on role
            string redirectUrl = user.Role switch
            {
                "ADMIN" => Url.Action("Index", "Admin"),
                "DOCTOR" => "https://localhost:7210/Doctors/Dashboard",
                "CUSTOMER" => Url.Action("Index", "Home"),
                _ => Url.Action("Index", "Home")
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

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
            {
                ViewBag.Error = "Invalid email!";
                return View();
            }

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "Email not found!";
                return View();
            }

            // Generate OTP for reset
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("ResetOTP", otp);
            HttpContext.Session.SetString("ResetEmail", email);
            HttpContext.Session.SetString("OTP_Expires", DateTime.UtcNow.AddMinutes(5).ToString("O"));

            var html = $"<h2>Your password reset OTP:</h2><h1 style='color:#dc3545'>{otp}</h1><p>Expires in 5 minutes.</p>";
            await _emailService.SendEmailAsync(email, "Reset Password", html);

            TempData["Success"] = "OTP has been sent to your email!";
            return RedirectToAction("VerifyResetOTP");
        }

        [HttpGet]
        public IActionResult VerifyResetOTP()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("ForgotPassword");

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public IActionResult VerifyResetOTP(string otp)
        {
            var savedOtp = HttpContext.Session.GetString("ResetOTP");
            var email = HttpContext.Session.GetString("ResetEmail");
            var expiresStr = HttpContext.Session.GetString("OTP_Expires");

            if (string.IsNullOrEmpty(savedOtp))
            {
                ViewBag.Error = "Session expired!";
                return RedirectToAction("ForgotPassword");
            }

            if (!DateTime.TryParse(expiresStr, out var expires) || DateTime.UtcNow > expires)
            {
                ClearResetSession();
                ViewBag.Error = "OTP has expired!";
                return RedirectToAction("ForgotPassword");
            }

            if (otp != savedOtp)
            {
                ViewBag.Email = email;
                ViewBag.Error = "Invalid OTP!";
                return View();
            }

            // OTP is valid → go to reset password page
            HttpContext.Session.SetString("ResetVerified", "true");
            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var verified = HttpContext.Session.GetString("ResetVerified");
            var email = HttpContext.Session.GetString("ResetEmail");

            if (verified != "true" || string.IsNullOrEmpty(email))
                return RedirectToAction("ForgotPassword");

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            var verified = HttpContext.Session.GetString("ResetVerified");

            if (verified != "true" || string.IsNullOrEmpty(email))
                return RedirectToAction("ForgotPassword");

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters!";
                ViewBag.Email = email;
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                ViewBag.Email = email;
                return View();
            }

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "Account not found!";
                return View();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userService.UpdateUserAsync(user);

            ClearResetSession();

            TempData["Message"] = "Password reset successfully! Please log in again.";
            return RedirectToAction("Login");
        }

        // Clear reset session data
        private void ClearResetSession()
        {
            HttpContext.Session.Remove("ResetOTP");
            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("OTP_Expires");
            HttpContext.Session.Remove("ResetVerified");
        }
    }
}
