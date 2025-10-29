using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        
        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
