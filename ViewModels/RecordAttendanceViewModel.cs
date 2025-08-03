using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels
{
    public class RecordAttendanceViewModel
    {
        [Required]
        [Display(Name = "QR Data")]
        public string QrData { get; set; }

        [Required]
        [Display(Name = "Schedule ID")]
        public int ScheduleId { get; set; }

        [Required]
        [Display(Name = "Booking ID")]
        public int BookingId { get; set; }

        [Required]
        [RegularExpression("^(checkin|checkout)$", ErrorMessage = "Mode must be either 'checkin' or 'checkout'")]
        [Display(Name = "Mode")]
        public string Mode { get; set; }
    }
}
