using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.AuthVMs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GYM_APP.Controllers
{
    public class AuthController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(GymDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UsersEmail == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UsersName = model.Name,
                    UsersEmail = model.Email,
                    UsersPhoneNumber = model.Phone,
                    UsersRole = "member",
                    UsersJoinedAt = DateTime.Now,
                    UsersPassword = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var member = new Member
                {
                    MemberAge = model.Age,
                    MemberWeightKg = model.Weight.HasValue ? (int?)Math.Round(model.Weight.Value) : null,
                    MembeHeightCm = model.Height.HasValue ? (int?)Math.Round(model.Height.Value) : null,
                    MemberHealthStatus = model.HealthIssues,
                    UsersId = user.UsersId
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Create authentication claims
                await SignInUserAsync(user);

                TempData["Success"] = "Registered successfully";
                return RedirectToAction("Profile", "Member");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during user registration");
                TempData["Error"] = "Something went wrong, please try again";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UsersEmail == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.UsersPassword))
            {
                // Check if the selected user type matches the user's actual role
                if (user.UsersRole.ToLower() != model.UserType.ToLower())
                {
                    ModelState.AddModelError("UserType", "Invalid user type selected for this account");
                    return View(model);
                }

                // Create authentication claims and sign in
                await SignInUserAsync(user);

                _logger.LogInformation($"User {user.UsersEmail} logged in successfully with role {user.UsersRole}");

                return RedirectUser(user.UsersRole);
            }

            ModelState.AddModelError("Email", "Email or password not correct, please try again");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsersId.ToString()),
                new Claim(ClaimTypes.Name, user.UsersName),
                new Claim(ClaimTypes.Email, user.UsersEmail),
                new Claim(ClaimTypes.Role, user.UsersRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Also set session for backward compatibility
            HttpContext.Session.SetInt32("UserId", user.UsersId);
            HttpContext.Session.SetString("UserRole", user.UsersRole);
        }

        private IActionResult RedirectUser(string role)
        {
            _logger.LogInformation($"Redirecting user with role: {role}");

            return role.ToLower() switch
            {
                "admin" => RedirectToAction("Dashboard", "Admin"),
                "trainer" => RedirectToAction("Trainer", "Admin"),
                "member" => RedirectToAction("Profile", "Member"),
                "assistant" => RedirectToAction("Index", "AttendancesList", new { area = "Admin" }),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}