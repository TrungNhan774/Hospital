using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalRazor.Pages.Doctors
{
    [Authorize(Roles = "DOCTOR")]
    public class DashboardModel : PageModel
    {
        public string DoctorName { get; set; } 
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int TotalRecords { get; set; }
        public int PendingPayments { get; set; }

        public string Appointment {  get; set; }
        public void OnGet()
        {
        }
    }
}
