using GYM_APP.Models;

namespace GYM_APP.ViewModels.AttendanceVMs
{
    public class AttendanceIndexViewModel
    {
        public List<Attendance> Attendances { get; set; } = new();
        public List<Class> Classes { get; set; } = new();
        public int TotalCheckins { get; set; }
        public int TotalCheckouts { get; set; }
        public double AttendanceRate { get; set; }
        public string MostActiveMember { get; set; } = string.Empty;

        // Filter properties
        public string? Member { get; set; }
        public int? ClassId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
