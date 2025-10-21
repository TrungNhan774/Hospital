using BLL.Services.Interfaces;
using DAL.Models;
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
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var users = await _userService.GetAllUsersAsync(searchString);
            ViewBag.SearchString = searchString;

            return View(users.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null) return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            // 🔹 Validate required fields
            if (string.IsNullOrWhiteSpace(user.Username))
                ModelState.AddModelError(nameof(user.Username), "Username is required.");

            if (string.IsNullOrWhiteSpace(user.Password))
                ModelState.AddModelError(nameof(user.Password), "Password is required.");

            if (string.IsNullOrWhiteSpace(user.Role))
                ModelState.AddModelError(nameof(user.Role), "Role is required.");

            if (!ModelState.IsValid)
                return View(user);

            try
            {
                // 🔹 Check for duplicate username/email
                var allUsers = await _userService.GetAllUsersAsync(null);
                bool usernameExists = allUsers.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
                bool emailExists = !string.IsNullOrEmpty(user.Email) &&
                                   allUsers.Any(u => !string.IsNullOrEmpty(u.Email) &&
                                                     u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

                if (usernameExists)
                    ModelState.AddModelError(nameof(user.Username), "Username already exists.");
                if (emailExists)
                    ModelState.AddModelError(nameof(user.Email), "Email already exists.");

                if (!ModelState.IsValid)
                    return View(user);

                user.CreatedAt = DateTime.Now;

                await _userService.CreateUserAsync(user);
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to create user. Username or email may already exist.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while creating the user.");
            }

            return View(user);
        }
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            try
            {
                if (id != user.UserId)
                {
                    ModelState.AddModelError("", "User ID mismatch.");
                    return View(user);
                }

                // Kiểm tra dữ liệu cơ bản
                if (string.IsNullOrWhiteSpace(user.Username))
                    ModelState.AddModelError(nameof(user.Username), "Username is required.");

                if (string.IsNullOrWhiteSpace(user.FullName))
                    ModelState.AddModelError(nameof(user.FullName), "Full name is required.");

                if (string.IsNullOrWhiteSpace(user.Email))
                    ModelState.AddModelError(nameof(user.Email), "Email is required.");

                if (string.IsNullOrWhiteSpace(user.Phone))
                    ModelState.AddModelError(nameof(user.Phone), "Phone number is required.");

                if (string.IsNullOrWhiteSpace(user.Role))
                    ModelState.AddModelError(nameof(user.Role), "Role is required.");

                if (!ModelState.IsValid)
                    return View(user);

                // Lấy user hiện tại
                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View(user);
                }

                // Giữ nguyên password nếu người dùng không nhập
                if (string.IsNullOrWhiteSpace(user.Password))
                    user.Password = existingUser.Password;

                // Kiểm tra trùng username / email
                var allUsers = await _userService.GetAllUsersAsync();
                bool usernameExists = allUsers.Any(u => u.UserId != user.UserId &&
                                                        u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
                bool emailExists = !string.IsNullOrEmpty(user.Email) &&
                                   allUsers.Any(u => u.UserId != user.UserId &&
                                                     !string.IsNullOrEmpty(u.Email) &&
                                                     u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

                if (usernameExists)
                    ModelState.AddModelError(nameof(user.Username), "Username already exists.");
                if (emailExists)
                    ModelState.AddModelError(nameof(user.Email), "Email already exists.");

                if (!ModelState.IsValid)
                    return View(user);

                // Cập nhật
                existingUser.Username = user.Username;
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.Role = user.Role;
                existingUser.Password = user.Password;

                await _userService.UpdateUserAsync(existingUser);

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Data concurrency error. Please try again.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Database update failed. Check unique fields like username or email.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ModelState.AddModelError("", "An unexpected error occurred while updating the user.");
            }

           
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
