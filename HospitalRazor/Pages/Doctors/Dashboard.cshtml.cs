using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalRazor.Pages.Doctors
{
    [Authorize(Roles = "DOCTOR")]
    public class DashboardModel : PageModel
    {
        private readonly IDoctorService _doctorService;

        public DashboardModel(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public string DoctorName { get; set; } = string.Empty;
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int TotalRecords { get; set; }

        // Dữ liệu cho biểu đồ
        public int CompletedAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CanceledAppointments { get; set; }

        public Doctor? Doctor { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);
            Doctor = await _doctorService.GetDoctorProfileByUserIdAsync(userId);
            if (Doctor == null)
                return NotFound();

            DoctorName = Doctor.FullName ?? "Unknown";

            var details = await _doctorService.GetDoctorStatisticsAsync(Doctor.DoctorId);
            if (details == null)
                return NotFound();

            TotalPatients = details.Appointments
                .Select(a => a.PatientId)
                .Distinct()
                .Count();

            var today = DateTime.Today;
            TodayAppointments = details.Appointments
                .Count(a => a.AppointmentDate.Date == today);

            TotalRecords = details.MedicalRecords.Count;

            // Biểu đồ: đếm số lượng từng trạng thái
            PendingAppointments = details.Appointments.Count(a => a.Status != null && a.Status.ToUpper() == "PENDING");
            CompletedAppointments = details.Appointments.Count(a => a.Status != null && a.Status.ToUpper() == "CONFIRMED");
            CanceledAppointments = details.Appointments.Count(a => a.Status != null && a.Status.ToUpper() == "CANCELLED");


            return Page();
        }
    }
}
