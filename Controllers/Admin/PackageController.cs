using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.PackageVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers.Admin
{
    public class PackageController : Controller
    {
        private readonly GymDbContext _context;

        public PackageController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var packages = await _context.Packages
                .Select(p => new PackageIndexViewModel
                {
                    PackagesId = p.PackagesId,
                    PackagesName = p.PackagesName ?? string.Empty,
                    PackagesDurationDays = p.PackagesDurationDays ?? 0,
                    PackagesPrice = p.PackagesPrice ?? 0,
                    PackagesType = p.PackagesType ?? string.Empty,
                    PackagesMaxFreezeDays = p.PackagesMaxFreezeDays ?? 0,
                    RefundPolicyDays = p.RefundPolicyDays ?? 0,
                    DailyChargeIfStarted = p.DailyChargeIfStarted ?? 0,
                    Privileges = p.Privileges
                })
                .ToListAsync();

            return View(packages);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var package = new Package
                {
                    PackagesName = viewModel.PackagesName,
                    PackagesDurationDays = viewModel.PackagesDurationDays,
                    PackagesPrice = viewModel.PackagesPrice,
                    PackagesType = viewModel.PackagesType,
                    PackagesMaxFreezeDays = viewModel.PackagesMaxFreezeDays,
                    RefundPolicyDays = viewModel.RefundPolicyDays,
                    DailyChargeIfStarted = viewModel.DailyChargeIfStarted,
                    Privileges = viewModel.Privileges
                };

                _context.Packages.Add(package);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Package created successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                TempData["Error"] = "Package not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new PackageEditViewModel
            {
                PackagesId = package.PackagesId,
                PackagesName = package.PackagesName ?? string.Empty,
                PackagesDurationDays = package.PackagesDurationDays ?? 0,
                PackagesPrice = package.PackagesPrice ?? 0,
                PackagesType = package.PackagesType ?? string.Empty,
                PackagesMaxFreezeDays = package.PackagesMaxFreezeDays ?? 0,
                RefundPolicyDays = package.RefundPolicyDays ?? 0,
                DailyChargeIfStarted = package.DailyChargeIfStarted ?? 0,
                Privileges = package.Privileges
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PackageEditViewModel viewModel)
        {
            if (id != viewModel.PackagesId)
            {
                TempData["Error"] = "Package ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var package = await _context.Packages.FindAsync(id);
                    if (package == null)
                    {
                        TempData["Error"] = "Package not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    package.PackagesName = viewModel.PackagesName;
                    package.PackagesDurationDays = viewModel.PackagesDurationDays;
                    package.PackagesPrice = viewModel.PackagesPrice;
                    package.PackagesType = viewModel.PackagesType;
                    package.PackagesMaxFreezeDays = viewModel.PackagesMaxFreezeDays;
                    package.RefundPolicyDays = viewModel.RefundPolicyDays;
                    package.DailyChargeIfStarted = viewModel.DailyChargeIfStarted;
                    package.Privileges = viewModel.Privileges;

                    _context.Update(package);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Package updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(viewModel.PackagesId))
                    {
                        TempData["Error"] = "Package not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                TempData["Error"] = "Package not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Package deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.PackagesId == id);
        }
    }
}