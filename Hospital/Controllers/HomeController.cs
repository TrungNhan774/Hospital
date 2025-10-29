using BLL.Services;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hospital.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IDepartmentService _departmentService;

        // Inject DoctorService và DepartmentService qua constructor
        public HomeController(IDoctorService doctorService, IDepartmentService departmentService)
        {
            _doctorService = doctorService;
            _departmentService = departmentService;
        }

        // ✅ Trang chủ
        public IActionResult Index()
        {
            return View();
        }

        // ✅ Trang đội ngũ bác sĩ
        [HttpGet]
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            ViewBag.Departments = _departmentService.GetAll();
            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> FilterDoctors(string searchString, int? departmentId)
        {
            var doctors = await _doctorService.GetDoctorsByDepartmentAsync(departmentId, searchString);
            return PartialView("_DoctorsListPartial", doctors);
        }

        // ✅ Trang danh sách chuyên khoa
        [HttpGet]
        public IActionResult Departments()
        {
            var departments = _departmentService.GetAll();
            return View(departments);
        }

        // ✅ Trang chi tiết chuyên khoa
        [HttpGet]
        public IActionResult DepartmentDetails(int id)
        {
            var department = _departmentService.GetById(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // ✅ Trang lỗi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
