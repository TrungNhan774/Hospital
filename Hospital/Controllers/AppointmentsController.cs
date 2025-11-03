using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Hospital.Controllers
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [Route("")]
        // GET: Appointments
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var appointments = await _appointmentService.GetAllAsync(searchString);
            ViewBag.SearchString = searchString;
            return View("~/Views/Admin/Appointments/Index.cshtml", appointments.ToPagedList(pageNumber, pageSize));
        }

        // GET: Appointments/Details/5
        [Route("Details/{id?}")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return View("~/Views/Admin/Appointments/Details.cshtml", appointment);
        }
    }
}
