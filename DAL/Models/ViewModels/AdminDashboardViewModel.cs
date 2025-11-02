using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalCustomers { get; set; }

        public List<TopDoctorViewModel> TopDoctors { get; set; } = new();
        public List<MonthlyRevenueViewModel> MonthlyRevenue { get; set; } = new();
        public List<PatientCountViewModel> PatientStats { get; set; } = new();
    }

    public class TopDoctorViewModel
    {
        public string DoctorName { get; set; }
        public int AppointmentCount { get; set; }
    }

    public class MonthlyRevenueViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class PatientCountViewModel
    {
        public string Period { get; set; } // "2025-11"
        public int TotalPatients { get; set; }
    }
}
