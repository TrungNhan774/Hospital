using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }
}
