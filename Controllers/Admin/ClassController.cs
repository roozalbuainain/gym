using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.ClassVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers.Admin
{
    [Route("admin/classes")]
    public class ClassController : Controller
    {
        private readonly GymDbContext _context;

        public ClassController(GymDbContext context)
        {
            _context = context;
        }

        // GET: admin/classes
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Include(c => c.Trainer)
                .ToListAsync();

            return View("~/Views/Admin/Class/Index.cshtml", classes);
        }

        // GET: admin/classes/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var trainers = await _context.Users
                .Where(u => u.UsersRole == "trainer")
                .ToListAsync();

            ViewBag.Trainers = trainers;
            return View("~/Views/Admin/Class/Create.cshtml");
        }

        // POST: admin/classes/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var classEntity = new Class
                {
                    ClassesName = model.ClassesName,
                    ClassesDescription = model.ClassesDescription,
                    ClassesDurationMinutes = model.ClassesDurationMinutes,
                    ClassesCapacity = model.ClassesCapacity,
                    TrainerId = model.TrainerId
                };

                _context.Classes.Add(classEntity);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Class created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // If model is invalid, reload trainers for the view
            var trainers = await _context.Users
                .Where(u => u.UsersRole == "trainer")
                .ToListAsync();

            ViewBag.Trainers = trainers;
            return View("~/Views/Admin/Class/Create.cshtml", model);
        }

        // GET: admin/classes/edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return NotFound();
            }

            var trainers = await _context.Users
                .Where(u => u.UsersRole == "trainer")
                .ToListAsync();

            var viewModel = new ClassEditViewModel
            {
                ClassesId = classEntity.ClassesId,
                ClassesName = classEntity.ClassesName ?? string.Empty,
                ClassesDescription = classEntity.ClassesDescription,
                ClassesDurationMinutes = classEntity.ClassesDurationMinutes ?? 0,
                ClassesCapacity = classEntity.ClassesCapacity ?? 0,
                TrainerId = classEntity.TrainerId ?? 0
            };

            ViewBag.Trainers = trainers;
            return View("~/Views/Admin/Class/Edit.cshtml", viewModel);
        }

        // POST: admin/classes/edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassEditViewModel model)
        {
            if (id != model.ClassesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var classEntity = await _context.Classes.FindAsync(id);
                    if (classEntity == null)
                    {
                        return NotFound();
                    }

                    classEntity.ClassesName = model.ClassesName;
                    classEntity.ClassesDescription = model.ClassesDescription;
                    classEntity.ClassesDurationMinutes = model.ClassesDurationMinutes;
                    classEntity.ClassesCapacity = model.ClassesCapacity;
                    classEntity.TrainerId = model.TrainerId;

                    _context.Update(classEntity);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Class updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(model.ClassesId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            // If model is invalid, reload trainers for the view
            var trainers = await _context.Users
                .Where(u => u.UsersRole == "trainer")
                .ToListAsync();

            ViewBag.Trainers = trainers;
            return View("~/Views/Admin/Class/Edit.cshtml", model);
        }

        // POST: admin/classes/delete/5
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Class deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassesId == id);
        }
    }
}