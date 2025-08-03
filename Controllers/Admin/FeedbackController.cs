using GYM_APP.ApplicationDbContext;
using GYM_APP.ViewModels.FeedbackVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers.Admin
{
    public class FeedbackController : Controller
    {
        private readonly GymDbContext _context;

        public FeedbackController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? classId)
        {
            var classes = await _context.Classes
                .Select(c => new ClassViewModel
                {
                    ClassesId = c.ClassesId,
                    ClassesName = c.ClassesName
                })
                .ToListAsync();

            var query = _context.Reviews
                .Include(r => r.Users)
                .Include(r => r.Class)
                .AsQueryable();

            if (classId.HasValue)
            {
                query = query.Where(r => r.ClassId == classId.Value);
            }

            var reviews = await query
                .OrderByDescending(r => r.ReviewsDate)
                .Select(r => new ReviewViewModel
                {
                    ReviewsId = r.ReviewsId,
                    ReviewsRating = r.ReviewsRating ?? 0,
                    ReviewsComment = r.ReviewsComment ?? string.Empty,
                    ReviewsDate = r.ReviewsDate ?? DateOnly.FromDateTime(DateTime.Now),
                    ReviewsTime = r.ReviewsTime ?? TimeOnly.FromDateTime(DateTime.Now),
                    UserName = r.Users.UsersName,
                    ClassName = r.Class.ClassesName,
                    ClassId = r.ClassId ?? 0,
                    UsersId = r.UsersId ?? 0
                })
                .ToListAsync();

            var viewModel = new FeedbackIndexViewModel
            {
                Reviews = reviews,
                Classes = classes,
                SelectedClassId = classId
            };

            return View("~/Views/Admin/Feedback/Index.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                TempData["Error"] = "Review not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Review deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}