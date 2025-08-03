using GYM_APP.ApplicationDbContext;
using GYM_APP.Models;
using GYM_APP.ViewModels.MemberVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GYM_APP.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly GymDbContext _context;

        public MemberController(GymDbContext context)
        {
            _context = context;
        }

        // GET: /Member/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users
                .Include(u => u.Members)
                .FirstOrDefaultAsync(u => u.UsersId == userId);

            if (user == null)
                return NotFound();

            var viewModel = new ProfileViewModel
            {
                UsersId = user.UsersId,
                Name = user.UsersName,
                Email = user.UsersEmail,
                Phone = user.UsersPhoneNumber,
                Age = user.Members.FirstOrDefault()?.MemberAge,
                HeightCm = user.Members.FirstOrDefault()?.MembeHeightCm,
                WeightKg = user.Members.FirstOrDefault()?.MemberWeightKg,
                BodySize = user.Members.FirstOrDefault()?.MemberBodySize,
                HealthStatus = user.Members.FirstOrDefault()?.MemberHealthStatus,
                FitnessGoals = user.Members.FirstOrDefault()?.MemberFitnessGoals
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var userId = GetCurrentUserId();
            var user = await _context.Users
                .Include(u => u.Members)
                .FirstOrDefaultAsync(u => u.UsersId == userId);

            if (user == null)
                return NotFound();

            // Check email uniqueness
            var emailExists = await _context.Users
                .AnyAsync(u => u.UsersEmail == model.Email && u.UsersId != userId);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already taken.");
                return View("Profile", model);
            }

            // Update user
            user.UsersName = model.Name;
            user.UsersEmail = model.Email;
            user.UsersPhoneNumber = model.Phone;

            // Update password if provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.UsersPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            // Update member info
            var member = user.Members.FirstOrDefault();
            if (member != null)
            {
                member.MemberAge = model.Age;
                member.MembeHeightCm = (int?)model.HeightCm;
                member.MemberWeightKg = (int?)model.WeightKg;
                member.MemberBodySize = model.BodySize;
                member.MemberHealthStatus = model.HealthStatus;
                member.MemberFitnessGoals = model.FitnessGoals;
            }
            else
            {
                member = new Member
                {
                    UsersId = userId,
                    MemberAge = model.Age,
                    MembeHeightCm = (int?)model.HeightCm,
                    MemberWeightKg = (int?)model.WeightKg,
                    MemberBodySize = model.BodySize,
                    MemberHealthStatus = model.HealthStatus,
                    MemberFitnessGoals = model.FitnessGoals
                };
                _context.Members.Add(member);
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        // GET: /Member/Payment/{packageId}
        public async Task<IActionResult> Payment(int packageId)
        {
            var package = await _context.Packages.FindAsync(packageId);
            if (package == null)
                return NotFound();

            var viewModel = new PaymentViewModel
            {
                PackageId = package.PackagesId,
                PackageName = package.PackagesName,
                PackagePrice = package.PackagesPrice ?? 0, // Handle nullable decimal
                PackageType = package.PackagesType,
                DurationDays = package.PackagesDurationDays ?? 0 // Handle nullable int
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(PaymentRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var package = await _context.Packages.FindAsync(model.PackageId);
            if (package == null)
                return NotFound();

            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            if (model.PaymentMethod == "cash")
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var subscription = new Subscription
                    {
                        SubscriptionsStartDate = DateOnly.FromDateTime(DateTime.Now),
                        SubscriptionsEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(package.PackagesDurationDays ?? 0)), // Fixed variable name
                        SubscriptionsStatus = "inactive",
                        PackagesId = package.PackagesId,
                        UsersId = userId
                    };

                    _context.Subscriptions.Add(subscription);
                    await _context.SaveChangesAsync();

                    var payment = new Payment
                    {
                        PaymentsAmouent = package.PackagesPrice,
                        PaymentsMethods = "cash",
                        PaymentsTransactionId = "TRX" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                        PaymentsStatus = "success",
                        PaymentsDate = DateOnly.FromDateTime(DateTime.Now),
                        PaymentsTime = TimeOnly.FromDateTime(DateTime.Now),
                        UsersId = userId,
                        SubscriptionsId = subscription.SubscriptionsId
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var successViewModel = new PaymentSuccessViewModel
                    {
                        PackageName = package.PackagesName,
                        PackagePrice = package.PackagesPrice ?? 0, // Handle nullable decimal
                        UserName = user.UsersName,
                        PaymentMethod = "cash"
                    };

                    return View("PaymentSuccess", successViewModel);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Something went wrong while processing your payment.";
                    return RedirectToAction("Payment", new { packageId = model.PackageId });
                }
            }

            // For card payment, generate OTP
            var otp = new Random().Next(100000, 999999);
            var cardLastFour = model.CardNumber?.Substring(Math.Max(0, model.CardNumber.Length - 4));

            var confirmViewModel = new ConfirmPaymentViewModel
            {
                PackageId = package.PackagesId,
                PackageName = package.PackagesName,
                PackagePrice = package.PackagesPrice ?? 0, // Handle nullable decimal
                UserName = user.UsersName,
                CardLastFour = cardLastFour,
                PaymentTime = DateTime.Now,
                Otp = otp
            };

            // Store OTP in session (you might want to use a more secure method)
            HttpContext.Session.SetInt32("otp", otp);
            HttpContext.Session.SetInt32("package_id", package.PackagesId);
            HttpContext.Session.SetString("card_last_four", cardLastFour);

            return View("ConfirmPayment", confirmViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sessionOtp = HttpContext.Session.GetInt32("otp");
            var sessionPackageId = HttpContext.Session.GetInt32("package_id");

            if (sessionOtp != model.Otp || sessionPackageId != model.PackageId)
            {
                TempData["Error"] = "Invalid OTP or session expired.";
                return RedirectToAction("Payment", new { packageId = model.PackageId });
            }

            var userId = GetCurrentUserId();
            var package = await _context.Packages.FindAsync(model.PackageId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var subscription = new Subscription
                {
                    SubscriptionsStartDate = DateOnly.FromDateTime(DateTime.Now),
                    SubscriptionsEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(package.PackagesDurationDays ?? 0)), // Fixed variable name
                    SubscriptionsStatus = "active",
                    PackagesId = package.PackagesId,
                    UsersId = userId
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                var payment = new Payment
                {
                    PaymentsAmouent = package.PackagesPrice,
                    PaymentsMethods = "card",
                    PaymentsTransactionId = "TRX" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                    PaymentsStatus = "success",
                    PaymentsDate = DateOnly.FromDateTime(DateTime.Now),
                    PaymentsTime = TimeOnly.FromDateTime(DateTime.Now),
                    UsersId = userId,
                    SubscriptionsId = subscription.SubscriptionsId
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Clear session
                HttpContext.Session.Remove("otp");
                HttpContext.Session.Remove("package_id");
                HttpContext.Session.Remove("card_last_four");

                TempData["Success"] = "Payment confirmed and subscription created successfully.";
                return RedirectToAction("PaymentSuccess");
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Something went wrong while processing your payment.";
                return RedirectToAction("Payment", new { packageId = model.PackageId });
            }
        }

        // GET: /Member/PaymentSuccess
        public IActionResult PaymentSuccess()
        {
            return View();
        }

        // GET: /Member/ListClasses
        public async Task<IActionResult> ListClasses()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            var latestSubscription = await _context.Subscriptions
                .Where(s => s.UsersId == userId)
                .OrderByDescending(s => s.SubscriptionsStartDate)
                .FirstOrDefaultAsync();

            if (latestSubscription == null || DateTime.Now > (latestSubscription.SubscriptionsEndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue)) // Fixed DateOnly conversion
            {
                TempData["Error"] = "🚫 You need an active subscription to book classes. Choose a package now to get started!";
                return RedirectToAction("Index", "Packages");
            }

            var classes = await _context.Classes
                .Include(c => c.ClassSchedules)
                .ThenInclude(cs => cs.Bookings)
                .Include(c => c.Trainer)
                .ToListAsync();

            var userBookings = await _context.Bookings
                .Where(b => b.UsersId == userId)
                .Select(b => b.ClassScheduleId)
                .ToListAsync();

            var viewModel = new BookClassesViewModel
            {
                Classes = classes.Select(c => new ClassViewModel
                {
                    ClassId = c.ClassesId,
                    ClassName = c.ClassesName,
                    ClassDescription = c.ClassesDescription,
                    Duration = c.ClassesDurationMinutes ?? 0, // Handle nullable int
                    Capacity = c.ClassesCapacity,
                    TrainerName = c.Trainer?.UsersName,
                    Schedules = c.ClassSchedules.Select(cs => new ClassScheduleViewModel
                    {
                        ScheduleId = cs.ClassScheduleId,
                        DayOfWeek = cs.ClassScheduleDayOfWeek,
                        Time = cs.ClassScheduleTime,
                        BookedCount = cs.Bookings.Count,
                        IsBooked = userBookings.Contains(cs.ClassScheduleId)
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /Member/BookClass
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookClass(BookClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var alreadyBooked = await _context.Bookings
                .AnyAsync(b => b.UsersId == userId && b.ClassScheduleId == model.ScheduleId);

            if (alreadyBooked)
            {
                TempData["Error"] = "You already booked this class.";
                return RedirectToAction("ListClasses");
            }

            var booking = new Booking
            {
                UsersId = userId,
                ClassScheduleId = model.ScheduleId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Class booked successfully!";
            return RedirectToAction("ListClasses");
        }

        // GET: /Member/MyClasses
        public async Task<IActionResult> MyClasses()
        {
            var userId = GetCurrentUserId();
            var bookedSchedules = await _context.ClassSchedules
                .Include(cs => cs.Classes)
                .Where(cs => cs.Bookings.Any(b => b.UsersId == userId))
                .GroupBy(cs => cs.ClassesId)
                .Select(g => new MyClassesViewModel
                {
                    ClassId = g.Key ?? 0,
                    ClassName = g.First().Classes.ClassesName,
                    ClassDescription = g.First().Classes.ClassesDescription,
                    Capacity = g.First().Classes.ClassesCapacity ?? 0, // Added capacity
                    Schedules = g.Select(cs => new ClassScheduleViewModel
                    {
                        ScheduleId = cs.ClassScheduleId,
                        DayOfWeek = cs.ClassScheduleDayOfWeek,
                        Time = cs.ClassScheduleTime
                    }).ToList()
                })
                .ToListAsync();

            return View(bookedSchedules);
        }

        // GET: /Member/MySubscriptions
        public async Task<IActionResult> MySubscriptions()
        {
            var userId = GetCurrentUserId();
            var subscriptions = await _context.Subscriptions
                .Include(s => s.Packages)
                .Where(s => s.UsersId == userId)
                .ToListAsync();

            var subscriptionViewModels = subscriptions.Select(s => new SubscriptionViewModel
            {
                SubscriptionId = s.SubscriptionsId,
                PackageName = s.Packages != null ? s.Packages.PackagesName : "",
                StartDate = s.SubscriptionsStartDate.HasValue ? s.SubscriptionsStartDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                EndDate = s.SubscriptionsEndDate.HasValue ? s.SubscriptionsEndDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                Status = s.SubscriptionsStatus,
                Price = s.Packages != null && s.Packages.PackagesPrice.HasValue ? s.Packages.PackagesPrice.Value : 0,
                FreezeStartDate = s.SubscriptionsFreezeStartDate.HasValue ? s.SubscriptionsFreezeStartDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                FreezeEndDate = s.SubscriptionsFreezeEndDate.HasValue ? s.SubscriptionsFreezeEndDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                DurationDays = s.Packages != null ? s.Packages.PackagesDurationDays ?? 0 : 0, // Added duration
                Privileges = s.Packages?.Privileges ?? "" // Added privileges
            }).ToList();

            return View(subscriptionViewModels);
        }

        // POST: /Member/CancelSubscription/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSubscription(int id)
        {
            var userId = GetCurrentUserId();
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.SubscriptionsId == id && s.UsersId == userId);

            if (subscription == null)
                return NotFound();

            subscription.SubscriptionsEndDate = DateOnly.FromDateTime(DateTime.Now);
            subscription.SubscriptionsStatus = "cancelled";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Subscription cancelled successfully.";
            return RedirectToAction("MySubscriptions");
        }

        // Add this to your MemberController.cs
        [HttpGet]
        public async Task<IActionResult> ClassDetails(int id)
        {
            var userId = GetCurrentUserId();

            // Get the class with its schedules, reviews, and trainer info
            var classItem = await _context.Classes
                .Include(c => c.ClassSchedules)
                .Include(c => c.Reviews)
                .ThenInclude(r => r.Users)
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.ClassesId == id);

            if (classItem == null)
            {
                return NotFound();
            }

            // Check if user is subscribed to any active package
            var isSubscribed = await _context.Subscriptions
                .AnyAsync(s => s.UsersId == userId &&
                              s.SubscriptionsEndDate >= DateOnly.FromDateTime(DateTime.Now));

            // Map to ViewModel
            var viewModel = new ClassViewModel
            {
                ClassId = classItem.ClassesId,
                ClassName = classItem.ClassesName,
                ClassDescription = classItem.ClassesDescription,
                Duration = classItem.ClassesDurationMinutes,
                Capacity = classItem.ClassesCapacity,
                TrainerName = classItem.Trainer?.UsersName,
                Schedules = classItem.ClassSchedules.Select(cs => new ClassScheduleViewModel
                {
                    ScheduleId = cs.ClassScheduleId,
                    DayOfWeek = cs.ClassScheduleDayOfWeek,
                    Time = cs.ClassScheduleTime
                }).ToList(),
                AvgRating = classItem.Reviews.Any() ? classItem.Reviews.Average(r => r.ReviewsRating) : null,
                Reviews = classItem.Reviews.Select(r => new ClassReviewViewModel
                {
                    UserName = r.Users?.UsersName,
                    Rating = r.ReviewsRating ?? 0,
                    Comment = r.ReviewsComment,
                    Date = r.ReviewsDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                    Time = r.ReviewsTime?.ToTimeSpan() ?? TimeSpan.Zero
                }).ToList(),
                IsSubscribed = isSubscribed
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int classId, int rating, string comment)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ClassDetails", new { id = classId });
            }

            var userId = GetCurrentUserId();

            // Verify user is subscribed
            var isSubscribed = await _context.Subscriptions
                .AnyAsync(s => s.UsersId == userId &&
                              s.SubscriptionsEndDate >= DateOnly.FromDateTime(DateTime.Now));

            if (!isSubscribed)
            {
                TempData["Error"] = "You must be subscribed to leave a review";
                return RedirectToAction("ClassDetails", new { id = classId });
            }

            // Check if user already reviewed this class
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ClassId == classId && r.UsersId == userId);

            if (existingReview != null)
            {
                TempData["Error"] = "You've already reviewed this class";
                return RedirectToAction("ClassDetails", new { id = classId });
            }

            // Add new review
            var review = new Review
            {
                ClassId = classId,
                UsersId = userId,
                ReviewsRating = rating,
                ReviewsComment = comment,
                ReviewsDate = DateOnly.FromDateTime(DateTime.Now),
                ReviewsTime = TimeOnly.FromDateTime(DateTime.Now)
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thank you for your review!";
            return RedirectToAction("ClassDetails", new { id = classId });
        }


        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}