using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.DTO
{
    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string? Status { get; set; }

    }
}
