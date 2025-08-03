using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.UsersVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/[controller]")]
    public class UserController : Controller
    {
        private readonly GymDbContext _context;

        public UserController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Admin/User
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Where(u => u.UsersRole != "Member")
                .OrderByDescending(u => u.UsersId)
                .Select(u => new UserIndexViewModel
                {
                    UsersId = u.UsersId,
                    UsersName = u.UsersName,
                    UsersEmail = u.UsersEmail,
                    UsersRole = u.UsersRole,
                    UsersPhoneNumber = u.UsersPhoneNumber,
                    UsersJoinedAt = u.UsersJoinedAt
                })
                .ToListAsync();

            return View(users);
        }

        // GET: Admin/User/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        // POST: Admin/User/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UsersEmail == model.UsersEmail);

                if (existingUser != null)
                {
                    ModelState.AddModelError("UsersEmail", "Email already exists.");
                    return View(model);
                }

                var user = new User
                {
                    UsersName = model.UsersName,
                    UsersEmail = model.UsersEmail,
                    UsersPassword = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    UsersRole = model.UsersRole,
                    UsersPhoneNumber = model.UsersPhoneNumber,
                    UsersJoinedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Admin/User/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                UsersId = user.UsersId,
                UsersName = user.UsersName,
                UsersEmail = user.UsersEmail,
                UsersRole = user.UsersRole,
                UsersPhoneNumber = user.UsersPhoneNumber
            };

            return View(model);
        }

        // POST: Admin/User/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.UsersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Check if email already exists (excluding current user)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UsersEmail == model.UsersEmail && u.UsersId != id);

                if (existingUser != null)
                {
                    ModelState.AddModelError("UsersEmail", "Email already exists.");
                    return View(model);
                }

                user.UsersName = model.UsersName;
                user.UsersEmail = model.UsersEmail;
                user.UsersRole = model.UsersRole;
                user.UsersPhoneNumber = model.UsersPhoneNumber;

                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.UsersPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // POST: Admin/User/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Members
        [HttpGet("Members")]
        public async Task<IActionResult> ListMembers(int page = 1, int pageSize = 10)
        {
            var totalMembers = await _context.Users
                .Where(u => u.UsersRole == "member")
                .CountAsync();

            var members = await _context.Users
                .Where(u => u.UsersRole == "member")
                .OrderByDescending(u => u.UsersId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserIndexViewModel
                {
                    UsersId = u.UsersId,
                    UsersName = u.UsersName,
                    UsersEmail = u.UsersEmail,
                    UsersRole = u.UsersRole,
                    UsersPhoneNumber = u.UsersPhoneNumber,
                    UsersJoinedAt = u.UsersJoinedAt
                })
                .ToListAsync();

            var viewModel = new MembersListViewModel
            {
                Members = members,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalMembers / pageSize),
                TotalMembers = totalMembers
            };

            return View(viewModel);
        }
    }


}