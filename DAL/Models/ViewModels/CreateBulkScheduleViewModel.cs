using System.ComponentModel.DataAnnotations;

namespace DAL.Models.ViewModels
{
    public class CreateBulkScheduleViewModel
    {
        [Required(ErrorMessage = "Please select a doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select a start date")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Please select an end date")]
        public DateOnly EndDate { get; set; }

        // MUST INITIALIZE = new List<string>()
        [MinLength(1, ErrorMessage = "Please select at least one shift")]
        public List<string> SelectedShifts { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "Please select at least one day of the week")]
        public List<string> SelectedDays { get; set; } = new List<string>();
    }
}