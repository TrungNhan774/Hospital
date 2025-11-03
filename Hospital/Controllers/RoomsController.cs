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
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: Rooms
        [Route("")]
        public async Task<IActionResult> Index(string? searchString, int? page)
        {
            var rooms = await _roomService.GetAllAsync();

            // Giữ lại chuỗi tìm kiếm để hiển thị lại trên view
            ViewBag.SearchString = searchString;

            // Lọc theo số phòng hoặc tên phòng (tùy thuộc model)
            if (!string.IsNullOrEmpty(searchString))
            {
                rooms = rooms
                    .Where(r => r.RoomNumber != null &&
                                r.RoomNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Phân trang
            int pageSize = 10; // Số dòng trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là 1

            return View("~/Views/Admin/Rooms/Index.cshtml", rooms.ToPagedList(pageNumber, pageSize));
        }

        // GET: Rooms/Details/5
        [Route("Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _roomService.GetByIdAsync(id.Value);
            if (room == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Rooms/Details.cshtml", room);
        }

        // GET: Admin/Rooms/Create
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["DepartmentId"] = new SelectList(await _roomService.GetDepartmentsAsyncRoom(), "DepartmentId", "Name");
            return View("~/Views/Admin/Rooms/Create.cshtml");
        }

        // POST: Admin/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("RoomId,DepartmentId,RoomNumber,Type")] Room room)
        {
            if (ModelState.IsValid)
            {
                // 🔹 Kiểm tra trùng
                var allRooms = await _roomService.GetAllAsync();
                bool exists = allRooms.Any(r =>
                    r.RoomNumber.Trim().Equals(room.RoomNumber.Trim(), StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    ModelState.AddModelError(nameof(room.RoomNumber), "This room number already exists.");
                    // reload dropdowns
                    ViewData["DepartmentId"] = new SelectList(await _roomService.GetDepartmentsAsyncRoom(), "DepartmentId", "Name", room.DepartmentId);
                    return View("~/Views/Admin/Rooms/Create.cshtml", room);
                }

                await _roomService.AddAsync(room);
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(await _roomService.GetDepartmentsAsyncRoom(), "DepartmentId", "Name", room.DepartmentId);
            return View("~/Views/Admin/Rooms/Create.cshtml", room);
        }


        // GET: Rooms/Edit/5
        [Route("Edit/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _roomService.GetByIdAsync(id.Value);
            if (room == null)
            {
                return NotFound();
            }

            // 🔹 Thêm ViewBag cho dropdown
            ViewBag.Types = new List<string> { "WARD", "SURGERY", "CONSULTATION" };

            ViewData["DepartmentId"] = new SelectList(await _roomService.GetDepartmentsAsyncRoom(), "DepartmentId", "Name", room.DepartmentId);
            return View("~/Views/Admin/Rooms/Edit.cshtml", room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,DepartmentId,RoomNumber,Type")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _roomService.UpdateAsync(room);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await RoomExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // 🔹 Khi ModelState invalid thì vẫn load lại dropdown
            ViewBag.Types = new List<string> { "WARD", "SURGERY", "CONSULTATION" };
            ViewData["DepartmentId"] = new SelectList(await _roomService.GetDepartmentsAsyncRoom(), "DepartmentId", "Name", room.DepartmentId);
            return View("~/Views/Admin/Rooms/Edit.cshtml", room);
        }

        // GET: Admin/Rooms/Delete/5
        [Route("Delete/{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _roomService.GetByIdAsync(id.Value);
            if (room == null) return NotFound();

            return View("~/Views/Admin/Rooms/Delete.cshtml", room);
        }

        // POST: Admin/Rooms/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            await _roomService.DeleteAsync(id);
            return Ok(); // ✅ Cho fetch() nhận biết xoá thành công
        }


        private async Task<bool> RoomExists(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            return room != null;
        }
    }
}
