using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.DTO
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public DateOnly WorkDate { get; set; }
        public string? Shift { get; set; }
        public bool? Available { get; set; }
    }
}
