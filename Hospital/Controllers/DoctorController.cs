using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Authorize(Roles = "DOCTOR")]
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}