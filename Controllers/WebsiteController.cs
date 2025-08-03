using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.WebsiteVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers
{
    public class WebsiteController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<WebsiteController> _logger;

        public WebsiteController(GymDbContext context, ILogger<WebsiteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Home()
        {
            try
            {
                _logger.LogInformation("Attempting to load classes");
                var classes = await _context.Classes
                    .Include(c => c.Trainer)
                    .ToListAsync();

                _logger.LogInformation($"Found {classes?.Count ?? 0} classes");

                var model = new HomeViewModel { Classes = classes ?? new List<Class>() };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading classes");
                return View(new HomeViewModel { Classes = new List<Class>() });
            }
        }

        public async Task<IActionResult> Packages()
        {
            var packages = await _context.Packages.ToListAsync();

            var model = new PackagesViewModel
            {
                Packages = packages
            };

            return View(model);
        }

        public async Task<IActionResult> Classes()
        {
            var classes = await _context.Classes
                .Include(c => c.ClassSchedules)
                .Include(c => c.Reviews)
                .Include(c => c.Trainer)
                .Select(c => new ClassViewModel
                {
                    ClassId = c.ClassesId,
                    ClassName = c.ClassesName,
                    ClassDescription = c.ClassesDescription,
                    DurationMinutes = c.ClassesDurationMinutes,
                    Capacity = c.ClassesCapacity,
                    Schedules = c.ClassSchedules.Select(s => new ClassScheduleViewModel
                    {
                        DayOfWeek = s.ClassScheduleDayOfWeek,
                        Time = s.ClassScheduleTime
                    }).ToList(),
                    AvgRating = c.Reviews.Any() ? c.Reviews.Average(r => r.ReviewsRating) : null
                })
                .ToListAsync();

            var model = new ClassesViewModel { Classes = classes };
            return View(model);
        }

        public async Task<IActionResult> ClassDetails(int id)
        {
            var classEntity = await _context.Classes
                .Include(c => c.ClassSchedules)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.Users)
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.ClassesId == id);

            if (classEntity == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            bool subscribed = false;

            if (userId.HasValue)
            {
                subscribed = await _context.Bookings
                    .Where(b => b.UsersId == userId.Value)
                    .AnyAsync(b => b.ClassSchedule.ClassesId == id);
            }

            // Calculate average rating
            var avgRating = classEntity.Reviews.Any()
                ? classEntity.Reviews.Average(r => r.ReviewsRating)
                : (double?)null;

            var model = new ClassDetailsViewModel
            {
                ClassId = classEntity.ClassesId,
                ClassName = classEntity.ClassesName,
                ClassDescription = classEntity.ClassesDescription,
                Capacity = classEntity.ClassesCapacity.GetValueOrDefault(),
                TrainerName = classEntity.Trainer?.UsersName,
                Schedules = classEntity.ClassSchedules.Select(s => new ClassScheduleViewModel
                {
                    ScheduleId = s.ClassScheduleId,
                    DayOfWeek = s.ClassScheduleDayOfWeek,
                    Time = s.ClassScheduleTime
                }).ToList(),
                Reviews = classEntity.Reviews.Select(r => new ClassReviewViewModel
                {
                    UserName = r.Users?.UsersName,
                    Rating = r.ReviewsRating ?? 0,
                    Comment = r.ReviewsComment,
                    Date = r.ReviewsDate.HasValue ?
                        new DateTime(r.ReviewsDate.Value.Year, r.ReviewsDate.Value.Month, r.ReviewsDate.Value.Day) :
                        DateTime.MinValue,
                    Time = r.ReviewsTime.HasValue ?
                        new TimeSpan(r.ReviewsTime.Value.Hour, r.ReviewsTime.Value.Minute, r.ReviewsTime.Value.Second) :
                        TimeSpan.Zero
                }).ToList(),
                AvgRating = avgRating,
                IsSubscribed = subscribed
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(AddReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid review data.";
                return RedirectToAction("ClassDetails", new { id = model.ClassId });
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "You must be logged in to add a review.";
                return RedirectToAction("Login", "Auth");
            }

            // Check if class exists
            var classExists = await _context.Classes
                .AnyAsync(c => c.ClassesId == model.ClassId);

            if (!classExists)
            {
                TempData["Error"] = "Class not found.";
                return RedirectToAction("Classes");
            }

            try
            {
                var review = new Review
                {
                    ReviewsRating = model.Rating,
                    ReviewsComment = model.Comment,
                    ReviewsDate = DateOnly.FromDateTime(DateTime.Now),
                    ReviewsTime = TimeOnly.FromDateTime(DateTime.Now),
                    ClassId = model.ClassId,
                    UsersId = userId.Value
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Review added successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                TempData["Error"] = "Something went wrong while adding your review.";
            }

            return RedirectToAction("ClassDetails", new { id = model.ClassId });
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }
    }
}