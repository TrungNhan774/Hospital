using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hospital.Controllers
{
    public class PatientsController : Controller
    {
        private readonly DbhospitalContext _context;

        public PatientsController(DbhospitalContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var patients = _context.Patients.ToList();
            return View(patients);
        }
    }
}
