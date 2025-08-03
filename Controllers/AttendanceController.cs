using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(GymDbContext context, ILogger<AttendanceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> ScanPage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var today = DateTime.Now.DayOfWeek.ToString();

            var bookings = await _context.Bookings
                .Include(b => b.ClassSchedule)
                    .ThenInclude(cs => cs.Classes)
                        .ThenInclude(c => c.Trainer)
                .Where(b => b.UsersId == userId.Value &&
                           b.ClassSchedule.ClassScheduleDayOfWeek == today)
                .ToListAsync();

            var model = new ScanPageViewModel
            {
                Bookings = bookings
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Record([FromBody] RecordAttendanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided" });
            }

            try
            {
                var qrParts = model.QrData.Split('|');
                if (qrParts.Length != 2)
                {
                    return Json(new { success = false, message = "Invalid QR code format" });
                }

                var userId = int.Parse(qrParts[0]);
                var type = qrParts[1];

                var today = DateTime.Today;
                var attendance = await _context.Attendances
                       .Where(a => a.UsersId == userId)
                       .Where(a => a.AttendancesTime.HasValue && a.AttendancesTime.Value.Date == DateTime.Now.Date)
                       .Where(a => a.AttendancesType == type)
                       .FirstOrDefaultAsync();

                if (model.Mode == "checkin")
                {
                    if (attendance == null)
                    {
                        attendance = new Attendance
                        {
                            AttendancesTime = DateTime.Now,
                            AttendancesCheckInTime = DateTime.Now,
                            AttendancesStatus = "Present",
                            AttendancesType = type,
                            UsersId = userId
                        };

                        _context.Attendances.Add(attendance);
                        await _context.SaveChangesAsync();

                        if (type == "member")
                        {
                            var memberAttendance = new MemberAttendance
                            {
                                AttendancesId = attendance.AttendancesId,
                                ClassScheduleId = model.ScheduleId,
                                BookingId = model.BookingId
                            };

                            _context.MemberAttendances.Add(memberAttendance);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                else if (model.Mode == "checkout")
                {
                    if (attendance != null && attendance.AttendancesCheckOutTime == null)
                    {
                        attendance.AttendancesCheckOutTime = DateTime.Now;
                        _context.Attendances.Update(attendance);
                        await _context.SaveChangesAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording attendance");
                return Json(new { success = false, message = "An error occurred while recording attendance" });
            }
        }

        public async Task<IActionResult> QrGenerator()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var today = DateTime.Now.DayOfWeek.ToString();

            var bookings = await _context.Bookings
                .Include(b => b.ClassSchedule)
                    .ThenInclude(cs => cs.Classes)
                        .ThenInclude(c => c.Trainer)
                .Where(b => b.UsersId == userId.Value &&
                           b.ClassSchedule.ClassScheduleDayOfWeek == today)
                .ToListAsync();

            var model = new QrGeneratorViewModel
            {
                Bookings = bookings,
                UserId = userId.Value
            };

            return View(model);
        }
    }
}