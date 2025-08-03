using System;
using System.Collections.Generic;
using GYM_APP.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_APP.ApplicationDbContext;

public partial class GymDbContext : DbContext
{
    public GymDbContext()
    {
    }

    public GymDbContext(DbContextOptions<GymDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassSchedule> ClassSchedules { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberAttendance> MemberAttendances { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-IEQV4DJ\\SQLSERVERDEV;Initial Catalog=gym;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendancesId);

            entity.ToTable("attendances");

            entity.HasIndex(e => e.UsersId, "IX_attendances_USERS_ID");

            entity.Property(e => e.AttendancesId).HasColumnName("ATTENDANCES_ID");
            entity.Property(e => e.AttendancesCheckInTime)
                .HasColumnType("datetime")
                .HasColumnName("ATTENDANCES_CHECK_IN_TIME");
            entity.Property(e => e.AttendancesCheckOutTime)
                .HasColumnType("datetime")
                .HasColumnName("ATTENDANCES_CHECK_OUT_TIME");
            entity.Property(e => e.AttendancesStatus)
                .HasMaxLength(50)
                .HasColumnName("ATTENDANCES_STATUS");
            entity.Property(e => e.AttendancesTime)
                .HasColumnType("datetime")
                .HasColumnName("ATTENDANCES_TIME");
            entity.Property(e => e.AttendancesType)
                .HasMaxLength(50)
                .HasColumnName("ATTENDANCES_TYPE");
            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");

            entity.HasOne(d => d.Users).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.UsersId)
                .HasConstraintName("FK_attendances_users");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("booking");

            entity.HasIndex(e => e.ClassScheduleId, "IX_booking_CLASS_SCHEDULE_ID");

            entity.HasIndex(e => e.UsersId, "IX_booking_USERS_ID");

            entity.Property(e => e.BookingId).HasColumnName("BOOKING_ID");
            entity.Property(e => e.ClassScheduleId).HasColumnName("CLASS_SCHEDULE_ID");
            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");

            entity.HasOne(d => d.ClassSchedule).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClassScheduleId)
                .HasConstraintName("FK_booking_class_schedule");

            entity.HasOne(d => d.Users).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UsersId)
                .HasConstraintName("FK_booking_users");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassesId);

            entity.ToTable("classes");

            entity.HasIndex(e => e.TrainerId, "IX_classes_TRAINER_ID");

            entity.Property(e => e.ClassesId).HasColumnName("CLASSES_ID");
            entity.Property(e => e.ClassesCapacity).HasColumnName("CLASSES_CAPACITY");
            entity.Property(e => e.ClassesDescription).HasColumnName("CLASSES_DESCRIPTION");
            entity.Property(e => e.ClassesDurationMinutes).HasColumnName("CLASSES_DURATION_MINUTES");
            entity.Property(e => e.ClassesName)
                .HasMaxLength(100)
                .HasColumnName("CLASSES_NAME");
            entity.Property(e => e.TrainerId).HasColumnName("TRAINER_ID");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK_classes_trainer");
        });

        modelBuilder.Entity<ClassSchedule>(entity =>
        {
            entity.ToTable("class_schedule");

            entity.HasIndex(e => e.ClassesId, "IX_class_schedule_CLASSES_ID");

            entity.Property(e => e.ClassScheduleId).HasColumnName("CLASS_SCHEDULE_ID");
            entity.Property(e => e.ClassScheduleDayOfWeek)
                .HasMaxLength(20)
                .HasColumnName("CLASS_SCHEDULE_DAY_OF_WEEK");
            entity.Property(e => e.ClassScheduleTime).HasColumnName("CLASS_SCHEDULE_TIME");
            entity.Property(e => e.ClassesId).HasColumnName("CLASSES_ID");

            entity.HasOne(d => d.Classes).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.ClassesId)
                .HasConstraintName("FK_class_schedule_classes");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("member");

            entity.HasIndex(e => e.UsersId, "IX_member_USERS_ID");

            entity.Property(e => e.MemberId).HasColumnName("MEMBER_ID");
            entity.Property(e => e.MembeHeightCm).HasColumnName("MEMBE_HEIGHT_CM");
            entity.Property(e => e.MemberAge).HasColumnName("MEMBER_AGE");
            entity.Property(e => e.MemberBodySize)
                .HasMaxLength(100)
                .HasColumnName("MEMBER_BODY_SIZE");
            entity.Property(e => e.MemberFitnessGoals).HasColumnName("MEMBER_FITNESS_GOALS");
            entity.Property(e => e.MemberHealthStatus)
                .HasMaxLength(255)
                .HasColumnName("MEMBER_HEALTH_STATUS");
            entity.Property(e => e.MemberWeightKg).HasColumnName("MEMBER_WEIGHT_KG");
            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");

            entity.HasOne(d => d.Users).WithMany(p => p.Members)
                .HasForeignKey(d => d.UsersId)
                .HasConstraintName("FK_member_users");
        });

        modelBuilder.Entity<MemberAttendance>(entity =>
        {
            entity.ToTable("member_attendance");

            entity.HasIndex(e => e.AttendancesId, "IX_member_attendance_ATTENDANCES_ID");

            entity.HasIndex(e => e.BookingId, "IX_member_attendance_BOOKING_ID");

            entity.HasIndex(e => e.ClassScheduleId, "IX_member_attendance_CLASS_SCHEDULE_ID");

            entity.Property(e => e.MemberAttendanceId).HasColumnName("MEMBER_ATTENDANCE_ID");
            entity.Property(e => e.AttendancesId).HasColumnName("ATTENDANCES_ID");
            entity.Property(e => e.BookingId).HasColumnName("BOOKING_ID");
            entity.Property(e => e.ClassScheduleId).HasColumnName("CLASS_SCHEDULE_ID");

            entity.HasOne(d => d.Attendances).WithMany(p => p.MemberAttendances)
                .HasForeignKey(d => d.AttendancesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_member_attendance_attendances");

            entity.HasOne(d => d.Booking).WithMany(p => p.MemberAttendances)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_member_attendance_booking");

            entity.HasOne(d => d.ClassSchedule).WithMany(p => p.MemberAttendances)
                .HasForeignKey(d => d.ClassScheduleId)
                .HasConstraintName("FK_member_attendance_class_schedule");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationsId);

            entity.ToTable("notifications");

            entity.HasIndex(e => e.UsersId, "IX_notifications_USERS_ID");

            entity.Property(e => e.NotificationsId).HasColumnName("NOTIFICATIONS_ID");
            entity.Property(e => e.NotificationsIsRead).HasColumnName("NOTIFICATIONS_IS_READ");
            entity.Property(e => e.NotificationsMessage).HasColumnName("NOTIFICATIONS_MESSAGE");
            entity.Property(e => e.NotificationsType)
                .HasMaxLength(50)
                .HasColumnName("NOTIFICATIONS_TYPE");
            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");

            entity.HasOne(d => d.Users).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UsersId)
                .HasConstraintName("FK_notifications_users");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackagesId);

            entity.ToTable("packages");

            entity.Property(e => e.PackagesId).HasColumnName("PACKAGES_ID");
            entity.Property(e => e.DailyChargeIfStarted)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DAILY_CHARGE_IF_STARTED");
            entity.Property(e => e.PackagesDurationDays).HasColumnName("PACKAGES_DURATION_DAYS");
            entity.Property(e => e.PackagesMaxFreezeDays).HasColumnName("PACKAGES_MAX_FREEZE_DAYS");
            entity.Property(e => e.PackagesName)
                .HasMaxLength(100)
                .HasColumnName("PACKAGES_NAME");
            entity.Property(e => e.PackagesPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PACKAGES_PRICE");
            entity.Property(e => e.PackagesType)
                .HasMaxLength(50)
                .HasColumnName("PACKAGES_TYPE");
            entity.Property(e => e.Privileges).HasColumnName("PRIVILEGES");
            entity.Property(e => e.RefundPolicyDays).HasColumnName("REFUND_POLICY_DAYS");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentsId);

            entity.ToTable("payments");

            entity.HasIndex(e => e.SubscriptionsId, "IX_payments_SUBSCRIPTIONS_ID");

            entity.HasIndex(e => e.UsersId, "IX_payments_USERS_ID");

            entity.Property(e => e.PaymentsId).HasColumnName("PAYMENTS_ID");
            entity.Property(e => e.PaymentsAmouent)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PAYMENTS_AMOUENT");
            entity.Property(e => e.PaymentsDate).HasColumnName("PAYMENTS_DATE");
            entity.Property(e => e.PaymentsMethods)
                .HasMaxLength(50)
                .HasColumnName("PAYMENTS_METHODS");
            entity.Property(e => e.PaymentsStatus)
                .HasMaxLength(50)
                .HasColumnName("PAYMENTS_STATUS");
            entity.Property(e => e.PaymentsTime).HasColumnName("PAYMENTS_TIME");
            entity.Property(e => e.PaymentsTransactionId)
                .HasMaxLength(100)
                .HasColumnName("PAYMENTS_TRANSACTION_ID");
            entity.Property(e => e.SubscriptionsId).HasColumnName("SUBSCRIPTIONS_ID");
            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");

            entity.HasOne(d => d.Subscriptions).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionsId)
                .HasConstraintName("FK_payments_subscriptions");

            entity.HasOne(d => d.Users).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UsersId)
                .HasConstraintName("FK_payments_users");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewsId);

            entity.ToTable("reviews");

            entity.HasIndex(e => e.ClassId, "IX_reviews_CLASS_ID");

            entity.HasIndex(e => e.UsersId, "IX_reviews_USERS_ID");

            entity.Property(e => e.ReviewsId).HasColumnName("REVIEWS_ID");
            entity.Property(e => e.ClassId).HasColumnName("CLASS_ID");
            entity.Property(e => e.ReviewsComment).HasColumnName("REVIEWS_COMMENT");
            entity.Property(e => e.ReviewsDate).HasColumnName("REVIEWS_DATE");
            entity.Property(e => e.ReviewsRating).HasColumnName("REVIEWS_RATING");
            entity.Property(e => e.ReviewsTime).HasColumnName("REVIEWS_TIME");
            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");

            entity.HasOne(d => d.Class).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_reviews_classes");

            entity.HasOne(d => d.Users).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UsersId)
                .HasConstraintName("FK_reviews_users");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionsId);

            entity.ToTable("subscriptions");

            entity.HasIndex(e => e.PackagesId, "IX_subscriptions_PACKAGES_ID");

            entity.HasIndex(e => e.UsersId, "IX_subscriptions_USERS_ID");

            entity.Property(e => e.SubscriptionsId).HasColumnName("SUBSCRIPTIONS_ID");
            entity.Property(e => e.PackagesId).HasColumnName("PACKAGES_ID");
            entity.Property(e => e.SubscriptionsEndDate).HasColumnName("SUBSCRIPTIONS_END_DATE");
            entity.Property(e => e.SubscriptionsFreezeEndDate).HasColumnName("SUBSCRIPTIONS_FREEZE_END_DATE");
            entity.Property(e => e.SubscriptionsFreezeStartDate).HasColumnName("SUBSCRIPTIONS_FREEZE_START_DATE");
            entity.Property(e => e.SubscriptionsStartDate).HasColumnName("SUBSCRIPTIONS_START_DATE");
            entity.Property(e => e.SubscriptionsStatus)
                .HasMaxLength(50)
                .HasColumnName("SUBSCRIPTIONS_STATUS");
            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");

            entity.HasOne(d => d.Packages).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PackagesId)
                .HasConstraintName("FK_subscriptions_packages");

            entity.HasOne(d => d.Users).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UsersId)
                .HasConstraintName("FK_subscriptions_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsersId);

            entity.ToTable("users");

            entity.HasIndex(e => e.UsersEmail, "UQ_users_USERS_EMAIL").IsUnique();

            entity.Property(e => e.UsersId).HasColumnName("USERS_ID");
            entity.Property(e => e.UsersEmail)
                .HasMaxLength(100)
                .HasColumnName("USERS_EMAIL");
            entity.Property(e => e.UsersJoinedAt)
                .HasColumnType("datetime")
                .HasColumnName("USERS_JOINED_AT");
            entity.Property(e => e.UsersName)
                .HasMaxLength(100)
                .HasColumnName("USERS_NAME");
            entity.Property(e => e.UsersPassword)
                .HasMaxLength(255)
                .HasColumnName("USERS_PASSWORD");
            entity.Property(e => e.UsersPhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("USERS_PHONE_NUMBER");
            entity.Property(e => e.UsersRole)
                .HasMaxLength(50)
                .HasColumnName("USERS_ROLE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
