using GYM_APP.ApplicationDbContext;
using GYM_APP.ViewModels.SubscribtionsVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.Controllers.Admin
{
    public class SubscriptionController : Controller
    {
        private readonly GymDbContext _context;

        public SubscriptionController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.Users)
                .Include(s => s.Packages)
                .Include(s => s.Payments)
                .Select(s => new SubscriptionIndexViewModel
                {
                    SubscriptionsId = s.SubscriptionsId,
                    SubscriptionsStartDate = s.SubscriptionsStartDate ?? DateOnly.FromDateTime(DateTime.Now),
                    SubscriptionsEndDate = s.SubscriptionsEndDate ?? DateOnly.FromDateTime(DateTime.Now),
                    SubscriptionsStatus = s.SubscriptionsStatus ?? string.Empty,
                    SubscriptionsFreezeStartDate = s.SubscriptionsFreezeStartDate,
                    SubscriptionsFreezeEndDate = s.SubscriptionsFreezeEndDate,
                    UserName = s.Users.UsersName ?? string.Empty,
                    UserEmail = s.Users.UsersEmail ?? string.Empty,
                    PackageName = s.Packages.PackagesName ?? string.Empty,
                    PackagePrice = s.Packages.PackagesPrice ?? 0,
                    TotalPayments = s.Payments.Sum(p => p.PaymentsAmouent ?? 0),
                    PaymentCount = s.Payments.Count(),
                    UsersId = s.UsersId ?? 0,
                    PackagesId = s.PackagesId ?? 0
                })
                .ToListAsync();

            return View(subscriptions);
        }

        public async Task<IActionResult> Invoice(int id)
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.Users)
                .Include(s => s.Packages)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.SubscriptionsId == id);

            if (subscription == null)
            {
                TempData["Error"] = "Subscription not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SubscriptionInvoiceViewModel
            {
                SubscriptionsId = subscription.SubscriptionsId,
                SubscriptionsStartDate = subscription.SubscriptionsStartDate ?? DateOnly.FromDateTime(DateTime.Now),
                SubscriptionsEndDate = subscription.SubscriptionsEndDate ?? DateOnly.FromDateTime(DateTime.Now),
                SubscriptionsStatus = subscription.SubscriptionsStatus ?? string.Empty,
                SubscriptionsFreezeStartDate = subscription.SubscriptionsFreezeStartDate,
                SubscriptionsFreezeEndDate = subscription.SubscriptionsFreezeEndDate,

                // User Details
                UserName = subscription.Users?.UsersName ?? string.Empty,
                UserEmail = subscription.Users?.UsersEmail ?? string.Empty,
                UserPhone = subscription.Users?.UsersPhoneNumber ?? string.Empty,
                UserJoinedAt = subscription.Users?.UsersJoinedAt ?? DateTime.Now,

                // Package Details
                PackageName = subscription.Packages?.PackagesName ?? string.Empty,
                PackagePrice = subscription.Packages?.PackagesPrice ?? 0,
                PackageType = subscription.Packages?.PackagesType ?? string.Empty,
                PackageDurationDays = subscription.Packages?.PackagesDurationDays ?? 0,
                PackageMaxFreezeDays = subscription.Packages?.PackagesMaxFreezeDays ?? 0,
                RefundPolicyDays = subscription.Packages?.RefundPolicyDays ?? 0,
                DailyChargeIfStarted = subscription.Packages?.DailyChargeIfStarted ?? 0,
                Privileges = subscription.Packages?.Privileges,

                // Payment Details
                Payments = subscription.Payments.Select(p => new PaymentViewModel
                {
                    PaymentsId = p.PaymentsId,
                    PaymentsAmount = p.PaymentsAmouent ?? 0,
                    PaymentsDate = p.PaymentsDate ?? DateOnly.FromDateTime(DateTime.Now),
                    PaymentsTime = p.PaymentsTime ?? TimeOnly.FromDateTime(DateTime.Now),
                    PaymentsMethods = p.PaymentsMethods ?? string.Empty,
                    PaymentsStatus = p.PaymentsStatus ?? string.Empty,
                    PaymentsTransactionId = p.PaymentsTransactionId ?? string.Empty
                }).ToList(),

                TotalAmountPaid = subscription.Payments.Sum(p => p.PaymentsAmouent ?? 0)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription == null)
            {
                TempData["Error"] = "Subscription not found.";
                return RedirectToAction(nameof(Index));
            }

            subscription.SubscriptionsStatus = "active";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Subscription activated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Freeze(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription == null)
            {
                TempData["Error"] = "Subscription not found.";
                return RedirectToAction(nameof(Index));
            }

            subscription.SubscriptionsStatus = "frozen";
            subscription.SubscriptionsFreezeStartDate = DateOnly.FromDateTime(DateTime.Now);
            subscription.SubscriptionsFreezeEndDate = null; // Clear any previous end date

            await _context.SaveChangesAsync();

            TempData["Success"] = "Subscription frozen successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfreeze(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription == null)
            {
                TempData["Error"] = "Subscription not found.";
                return RedirectToAction(nameof(Index));
            }

            subscription.SubscriptionsStatus = "active";
            subscription.SubscriptionsFreezeEndDate = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Subscription reactivated (unfrozen) successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}