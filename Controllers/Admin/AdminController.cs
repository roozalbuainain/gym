using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.AdminVMs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GYM_APP.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(GymDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var members = await _context.Members.CountAsync();
                var subscriptionCount = await _context.Subscriptions.CountAsync();
                var packageCount = await _context.Packages.CountAsync();

                var currentYear = DateTime.Now.Year;
                var subscriptionsByMonth = await _context.Subscriptions
                    .Where(s => s.SubscriptionsStartDate.HasValue && s.SubscriptionsStartDate.Value.Year == currentYear)
                    .GroupBy(s => s.SubscriptionsStartDate.Value.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Month)
                    .ToListAsync();

                var labels = new List<string>();
                var data = new List<int>();

                for (int m = 1; m <= 12; m++)
                {
                    var monthLabel = new DateTime(currentYear, m, 1).ToString("MMM");
                    labels.Add(monthLabel);

                    var record = subscriptionsByMonth.FirstOrDefault(x => x.Month == m);
                    data.Add(record?.Count ?? 0);
                }

                var monthlySubscriptions = new
                {
                    Labels = labels,
                    Data = data
                };

                var dashboardViewModel = new DashboardViewModel
                {
                    Members = members,
                    SubscriptionCount = subscriptionCount,
                    PackageCount = packageCount,
                    MonthlySubscriptions = monthlySubscriptions
                };

                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = "Error loading dashboard data";
                return View(new DashboardViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.UsersId == userId);

                if (admin == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var profileViewModel = new AdminProfileViewModel
                {
                    UsersName = admin.UsersName,
                    UsersEmail = admin.UsersEmail,
                    UsersPhoneNumber = admin.UsersPhoneNumber
                };

                return View(profileViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin profile");
                TempData["Error"] = "Error loading profile";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditProfile", model);
            }

            try
            {
                var userId = GetCurrentUserId();
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.UsersId == userId);

                if (admin == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Check if email already exists for another user
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UsersEmail == model.UsersEmail && u.UsersId != userId);

                if (existingUser != null)
                {
                    ModelState.AddModelError("UsersEmail", "Email already exists");
                    return View("EditProfile", model);
                }

                // Update admin details
                admin.UsersName = model.UsersName;
                admin.UsersEmail = model.UsersEmail;
                admin.UsersPhoneNumber = model.UsersPhoneNumber;

                // Update password if provided
                if (!string.IsNullOrEmpty(model.UsersPassword))
                {
                    admin.UsersPassword = BCrypt.Net.BCrypt.HashPassword(model.UsersPassword);
                }

                await _context.SaveChangesAsync();

                // Update claims if email changed
                if (User.FindFirst(ClaimTypes.Email)?.Value != model.UsersEmail)
                {
                    await UpdateUserClaims(admin);
                }

                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction("EditProfile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin profile");
                TempData["Error"] = "Error updating profile. Please try again.";
                return View("EditProfile", model);
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        private async Task UpdateUserClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsersId.ToString()),
                new Claim(ClaimTypes.Name, user.UsersName),
                new Claim(ClaimTypes.Email, user.UsersEmail),
                new Claim(ClaimTypes.Role, user.UsersRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}