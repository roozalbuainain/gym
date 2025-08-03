using GYM_APP.ApplicationDbContext;
using GYM_APP.ViewModels.TrainerVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GYM_APP.Controllers.Admin
{
    [Authorize(Roles = "trainer")]
    [Route("Admin/[controller]")]
    public class TrainerController : Controller
    {
        private readonly GymDbContext _context;

        public TrainerController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Trainer
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var trainerId = GetCurrentUserId();

            var classes = await _context.Classes
                .Where(c => c.TrainerId == trainerId)
                .Include(c => c.ClassSchedules)
                .ThenInclude(cs => cs.Bookings)
                .Select(c => new TrainerClassViewModel
                {
                    ClassesId = c.ClassesId,
                    ClassesName = c.ClassesName ?? string.Empty,
                    ClassesDescription = c.ClassesDescription,
                    ClassesCapacity = c.ClassesCapacity ?? 0,
                    ClassesDurationMinutes = c.ClassesDurationMinutes ?? 0,
                    SchedulesCount = c.ClassSchedules.Count(),
                    BookingsCount = c.ClassSchedules.Sum(cs => cs.Bookings.Count)
                })
                .ToListAsync();

            var schedules = await _context.ClassSchedules
                .Where(cs => cs.Classes.TrainerId == trainerId)
                .Include(cs => cs.Classes)
                .Include(cs => cs.Bookings)
                .OrderBy(cs => cs.ClassScheduleTime)
                .Select(cs => new TrainerScheduleViewModel
                {
                    ClassScheduleId = cs.ClassScheduleId,
                    ClassScheduleDayOfWeek = cs.ClassScheduleDayOfWeek ?? string.Empty,
                    ClassScheduleTime = cs.ClassScheduleTime ?? TimeOnly.MinValue,
                    ClassName = cs.Classes.ClassesName ?? string.Empty,
                    ClassDuration = cs.Classes.ClassesDurationMinutes ?? 0,
                    ClassCapacity = cs.Classes.ClassesCapacity ?? 0,
                    BookingsCount = cs.Bookings.Count
                })
                .ToListAsync();

            var viewModel = new TrainerHomeViewModel
            {
                Classes = classes,
                Schedules = schedules
            };

            return View(viewModel);
        }

        // GET: Admin/Trainer/Calendar
        [HttpGet("Calendar")]
        public async Task<IActionResult> Calendar()
        {
            var trainerId = GetCurrentUserId();

            var classes = await _context.Classes
                .Where(c => c.TrainerId == trainerId)
                .Include(c => c.ClassSchedules)
                .Select(c => new TrainerClassViewModel
                {
                    ClassesId = c.ClassesId,
                    ClassesName = c.ClassesName ?? string.Empty,
                    ClassesDescription = c.ClassesDescription,
                    ClassesCapacity = c.ClassesCapacity ?? 0,
                    ClassesDurationMinutes = c.ClassesDurationMinutes ?? 0,
                    SchedulesCount = c.ClassSchedules.Count()
                })
                .ToListAsync();

            var schedules = await _context.ClassSchedules
                .Where(cs => cs.Classes.TrainerId == trainerId)
                .Include(cs => cs.Classes)
                .OrderBy(cs => cs.ClassScheduleTime)
                .Select(cs => new TrainerScheduleViewModel
                {
                    ClassScheduleId = cs.ClassScheduleId,
                    ClassScheduleDayOfWeek = cs.ClassScheduleDayOfWeek ?? string.Empty,
                    ClassScheduleTime = cs.ClassScheduleTime ?? TimeOnly.MinValue,
                    ClassName = cs.Classes.ClassesName ?? string.Empty,
                    ClassDuration = cs.Classes.ClassesDurationMinutes ?? 0,
                    ClassCapacity = cs.Classes.ClassesCapacity ?? 0
                })
                .ToListAsync();

            var viewModel = new TrainerCalendarViewModel
            {
                Classes = classes,
                Schedules = schedules
            };

            return View(viewModel);
        }

        // GET: Admin/Trainer/Classes
        [HttpGet("Classes")]
        public async Task<IActionResult> Classes()
        {
            var trainerId = GetCurrentUserId();

            var classes = await _context.Classes
                .Where(c => c.TrainerId == trainerId)
                .Include(c => c.ClassSchedules)
                .Select(c => new TrainerClassViewModel
                {
                    ClassesId = c.ClassesId,
                    ClassesName = c.ClassesName ?? string.Empty,
                    ClassesDescription = c.ClassesDescription,
                    ClassesCapacity = c.ClassesCapacity ?? 0,
                    ClassesDurationMinutes = c.ClassesDurationMinutes ?? 0,
                    SchedulesCount = c.ClassSchedules.Count()
                })
                .ToListAsync();

            var viewModel = new TrainerClassesViewModel
            {
                Classes = classes
            };

            return View(viewModel);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }


}