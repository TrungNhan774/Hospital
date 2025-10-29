using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalRazor.Pages.Doctors
{
    [Authorize(Roles = "DOCTOR")]
    public class ProfileModel : PageModel
    {
        private readonly IDoctorService _doctorService;

        public ProfileModel(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public Doctor? Doctor { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy UserId từ claim của tài khoản đăng nhập
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToPage("/Account/Login");

            int userId = int.Parse(userIdClaim);

            // Lấy hồ sơ bác sĩ theo UserId
            Doctor = await _doctorService.GetDoctorProfileByUserIdAsync(userId);

            if (Doctor == null)
                return NotFound();

            return Page();
        }
    }
}
