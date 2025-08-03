using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.AttendanceVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GYM_APP.Controllers.Admin
{
    public class AttendanceController : Controller
    {
        private readonly GymDbContext _context;

        public AttendanceController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? member, int? classId, DateTime? from, DateTime? to)
        {
            var classes = await _context.Classes.ToListAsync();

            var query = _context.Attendances
                .Include(a => a.Users)
                .Include(a => a.MemberAttendances)
                    .ThenInclude(ma => ma.ClassSchedule)
                        .ThenInclude(cs => cs.Classes)
                .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(member))
            {
                query = query.Where(a => a.Users.UsersName.Contains(member));
            }

            if (classId.HasValue)
            {
                query = query.Where(a => a.MemberAttendances.Any(ma =>
                    ma.ClassSchedule.ClassesId == classId.Value));
            }

            if (from.HasValue)
            {
                query = query.Where(a => a.AttendancesTime.HasValue && a.AttendancesTime.Value.Date >= from.Value.Date);
            }

            if (to.HasValue)
            {
                query = query.Where(a => a.AttendancesTime.HasValue && a.AttendancesTime.Value.Date <= to.Value.Date);
            }

            var attendances = await query
                .OrderByDescending(a => a.AttendancesTime)
                .ToListAsync();

            // Statistics
            var totalCheckins = attendances.Count(a => a.AttendancesCheckInTime != null);
            var totalCheckouts = attendances.Count(a => a.AttendancesCheckOutTime != null);
            var totalRecords = attendances.Count;
            var attendanceRate = totalRecords > 0 ? Math.Round((double)totalCheckins / totalRecords * 100, 2) : 0;

            // Most active member
            var mostActiveUserId = attendances
                .GroupBy(a => a.UsersId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            var mostActiveMember = "N/A";
            if (mostActiveUserId != null)
            {
                var user = await _context.Users.FindAsync(mostActiveUserId);
                mostActiveMember = user?.UsersName ?? "N/A";
            }

            var viewModel = new AttendanceIndexViewModel
            {
                Attendances = attendances,
                Classes = classes,
                TotalCheckins = totalCheckins,
                TotalCheckouts = totalCheckouts,
                AttendanceRate = attendanceRate,
                MostActiveMember = mostActiveMember,
                // Filter values for the view
                Member = member,
                ClassId = classId,
                From = from,
                To = to
            };

            return View("~/Views/Admin/Attendance/Index.cshtml", viewModel);
        }

        public IActionResult Create()
        {
            var userId = GetCurrentUserId();
            var nextAction = GetNextAction(userId);

            var viewModel = new AttendanceCreateViewModel
            {
                NextAction = nextAction
            };

            return View("~/Views/Admin/Attendance/Create.cshtml", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Check(int userId)
        {
            var today = DateTime.Today;

            var checkInToday = await _context.Attendances
                .Where(a => a.UsersId == userId &&
                           a.AttendancesTime.HasValue &&
                           a.AttendancesTime.Value.Date == today &&
                           a.AttendancesCheckInTime != null)
                .AnyAsync();

            var checkOutToday = await _context.Attendances
                .Where(a => a.UsersId == userId &&
                           a.AttendancesTime.HasValue &&
                           a.AttendancesTime.Value.Date == today &&
                           a.AttendancesCheckOutTime != null)
                .AnyAsync();

            return Json(new
            {
                checkInToday = checkInToday,
                checkOutToday = checkOutToday
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(AttendanceStoreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data provided.";
                return RedirectToAction("Create");
            }

            var userExists = await _context.Users.AnyAsync(u => u.UsersId == model.UsersId);
            if (!userExists)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Create");
            }

            var today = DateTime.Today;
            var attendance = await _context.Attendances
                .Where(a => a.UsersId == model.UsersId &&
                           a.AttendancesTime.HasValue &&
                           a.AttendancesTime.Value.Date == today)
                .FirstOrDefaultAsync();

            if (attendance == null)
            {
                // Create new attendance record
                attendance = new Attendance
                {
                    AttendancesTime = DateTime.Now,
                    AttendancesCheckInTime = DateTime.Now,
                    AttendancesStatus = "Present",
                    AttendancesType = "employee",
                    UsersId = model.UsersId
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                // Update existing record with checkout time
                attendance.AttendancesCheckOutTime = DateTime.Now;
                _context.Attendances.Update(attendance);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Attendance recorded successfully";
            return RedirectToAction("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Scan(AttendanceScanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data provided.";
                return RedirectToAction("Create");
            }

            var userExists = await _context.Users.AnyAsync(u => u.UsersId == model.UserId);
            if (!userExists)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Create");
            }

            var today = DateTime.Today;

            // Check if already performed this action today
            var exists = await _context.Attendances
                .Where(a => a.UsersId == model.UserId &&
                           a.AttendancesType == model.Type &&
                           a.AttendancesTime.HasValue &&
                           a.AttendancesTime.Value.Date == today)
                .AnyAsync();

            if (exists)
            {
                TempData["Error"] = $"You have already {model.Type} today.";
                return RedirectToAction("Create");
            }

            // Check if trying to check out without checking in
            if (model.Type == "check-out")
            {
                var hasCheckedIn = await _context.Attendances
                    .Where(a => a.UsersId == model.UserId &&
                               a.AttendancesType == "check-in" &&
                               a.AttendancesTime.HasValue &&
                               a.AttendancesTime.Value.Date == today)
                    .AnyAsync();

                if (!hasCheckedIn)
                {
                    TempData["Error"] = "You must check in before checking out.";
                    return RedirectToAction("Create");
                }
            }

            // Create attendance record
            var attendance = new Attendance
            {
                UsersId = model.UserId,
                AttendancesType = model.Type,
                AttendancesTime = DateTime.Now,
                AttendancesCheckInTime = model.Type == "check-in" ? DateTime.Now : null,
                AttendancesCheckOutTime = model.Type == "check-out" ? DateTime.Now : null,
                AttendancesStatus = "completed"
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Attendance recorded via QR code";
            return RedirectToAction("Create");
        }

        private string GetNextAction(int userId)
        {
            var today = DateTime.Today;

            var checkInToday = _context.Attendances
                .Where(a => a.UsersId == userId &&
                           a.AttendancesType == "check-in" &&
                           a.AttendancesTime.HasValue &&
                           a.AttendancesTime.Value.Date == today)
                .Any();

            var checkOutToday = _context.Attendances
                .Where(a => a.UsersId == userId &&
                           a.AttendancesType == "check-out" &&
                           a.AttendancesTime.HasValue &&
                           a.AttendancesTime.Value.Date == today)
                .Any();

            if (!checkInToday)
                return "check-in";
            else if (!checkOutToday)
                return "check-out";
            else
                return null; // Both actions completed
        }

        private int GetCurrentUserId()
        {
            // Get the current user's ID from the authentication context
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Fallback - you might want to handle this differently
            return 1; // or throw an exception
        }
    }
}