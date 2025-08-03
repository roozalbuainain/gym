using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.AdminVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers.Admin
{
    public class ClassScheduleController : Controller
    {
        private readonly GymDbContext _context;

        public ClassScheduleController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ClassSchedule
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var schedules = await _context.ClassSchedules
                .Include(s => s.Classes)
                .ToListAsync();
            return View("../Admin/ClassSchedule/Index", schedules);
        }

        // GET: Admin/ClassSchedule/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View("../Admin/ClassSchedule/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var schedule = new ClassSchedule
                {
                    ClassScheduleDayOfWeek = model.ClassScheduleDayOfWeek,
                    ClassScheduleTime = model.ClassScheduleTime,
                    ClassesId = model.ClassesId
                };

                _context.ClassSchedules.Add(schedule);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Schedule added successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View("../Admin/ClassSchedule/Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View("../Admin/ClassSchedule/Edit", new ClassScheduleViewModel
            {
                ClassScheduleId = schedule.ClassScheduleId,
                ClassScheduleDayOfWeek = schedule.ClassScheduleDayOfWeek,
                ClassScheduleTime = schedule.ClassScheduleTime,
                ClassesId = schedule.ClassesId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassScheduleViewModel model)
        {
            if (id != model.ClassScheduleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var schedule = await _context.ClassSchedules.FindAsync(id);
                    if (schedule == null) return NotFound();

                    schedule.ClassScheduleDayOfWeek = model.ClassScheduleDayOfWeek;
                    schedule.ClassScheduleTime = model.ClassScheduleTime;
                    schedule.ClassesId = model.ClassesId;

                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Schedule updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassScheduleExists(model.ClassScheduleId)) return NotFound();
                    throw;
                }
            }

            ViewBag.Classes = await _context.Classes.ToListAsync();
            return View("../Admin/ClassSchedule/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            _context.ClassSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Schedule deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/ClassSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.ClassSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Schedule deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ClassScheduleExists(int id)
        {
            return _context.ClassSchedules.Any(e => e.ClassScheduleId == id);
        }
    }
}