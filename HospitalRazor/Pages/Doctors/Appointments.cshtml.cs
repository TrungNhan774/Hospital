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
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }


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

            //  Lọc trạng thái
            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter.ToUpper() != "ALL")
                appointments = appointments.Where(a => a.Status?.ToUpper() == StatusFilter.ToUpper()).ToList();

            //  Lọc theo khoảng ngày
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var start = StartDate.Value.Date;
                var end = EndDate.Value.Date.AddDays(1); // lấy hết ngày EndDate

                appointments = appointments
                    .Where(a => a.AppointmentDate >= start && a.AppointmentDate < end)
                    .ToList();
            }
            else if (StartDate.HasValue)
            {
                var start = StartDate.Value.Date;
                appointments = appointments
                    .Where(a => a.AppointmentDate >= start)
                    .ToList();
            }
            else if (EndDate.HasValue)
            {
                var end = EndDate.Value.Date.AddDays(1);
                appointments = appointments
                    .Where(a => a.AppointmentDate < end)
                    .ToList();
            }


            Appointments = appointments.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync(int appointmentId)
        {
            await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, "CONFIRMED");
            return RedirectToPage(new { StatusFilter, StartDate, EndDate });
        }

        public async Task<IActionResult> OnPostCancelAsync(int appointmentId)
        {
            await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, "CANCELLED");
            return RedirectToPage(new { StatusFilter, StartDate, EndDate });
        }
    }
}
