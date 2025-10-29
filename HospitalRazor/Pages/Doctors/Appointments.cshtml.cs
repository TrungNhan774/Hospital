using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalRazor.Pages.Doctors
{
    [Authorize(Roles = "DOCTOR")]
    public class AppointmentsModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;

        public AppointmentsModel(IAppointmentService appointmentService, IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
        }

        public Doctor? Doctor { get; set; }
        public List<Appointment>? Appointments { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateFilter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToPage("/Account/Login");

            int userId = int.Parse(userIdClaim);
            Doctor = await _doctorService.GetDoctorProfileByUserIdAsync(userId);
            if (Doctor == null)
                return Content("Invalid Doctor ID");

            var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(Doctor.DoctorId);

            // ✅ Lọc trạng thái
            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
                appointments = appointments.Where(a => a.Status == StatusFilter).ToList();

            // ✅ Lọc theo ngày
            if (DateFilter.HasValue)
                appointments = appointments.Where(a => a.AppointmentDate.Date == DateFilter.Value.Date).ToList();

            Appointments = appointments.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync(int appointmentId)
        {
            await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, "CONFIRMED");
            return RedirectToPage(new { StatusFilter, DateFilter });
        }

        public async Task<IActionResult> OnPostCancelAsync(int appointmentId)
        {
            await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, "CANCELLED");
            return RedirectToPage(new { StatusFilter, DateFilter });
        }
    }
}
