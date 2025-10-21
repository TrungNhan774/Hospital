using BLL.Services;
using DAL.Models;
using DAL.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("Admin/[controller]")]
    public class AdminDepartmentController : Controller
    {
        private readonly IDepartmentService _service;

        public AdminDepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var departments = _service.GetAll()
                .Select(d => new DepartmentDTO
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Description = d.Description,
                    DoctorCount = d.Doctors.Count,
                    RoomCount = d.Rooms.Count
                }).ToList();

            return View("~/Views/Admin/Departments/Index.cshtml", departments);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Departments/Create.cshtml", new DepartmentDTO());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentDTO dto)
        {
            if (ModelState.IsValid)
            {
                var dep = new Department
                {
                    Name = dto.Name,
                    Description = dto.Description
                };
                _service.Add(dep);
                TempData["Success"] = "✅ Department created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "⚠️ Please fix the errors in the form.";
            return View("~/Views/Admin/Departments/Create.cshtml", dto);
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var dep = _service.GetById(id);
            if (dep == null) return NotFound();

            var dto = new DepartmentDTO
            {
                DepartmentId = dep.DepartmentId,
                Name = dep.Name,
                Description = dep.Description
            };

            return View("~/Views/Admin/Departments/Edit.cshtml", dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DepartmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "⚠️ Please fix the errors in the form.";
                return View("~/Views/Admin/Departments/Edit.cshtml", dto);
            }

            var dep = new Department
            {
                DepartmentId = dto.DepartmentId,
                Name = dto.Name,
                Description = dto.Description
            };
            _service.Update(dep);
            TempData["Success"] = "✅ Department updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var dep = _service.GetById(id);
            if (dep == null) return NotFound();

            var dto = new DepartmentDTO
            {
                DepartmentId = dep.DepartmentId,
                Name = dep.Name,
                Description = dep.Description,
                DoctorCount = dep.Doctors.Count,
                RoomCount = dep.Rooms.Count
            };

            return View("~/Views/Admin/Departments/Details.cshtml", dto);
        }

        [HttpPost("DeleteConfirmed/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _service.Delete(id);
            TempData["Success"] = "✅ Department deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
