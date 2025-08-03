
using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.MemberVMs
{
    public class BookClassViewModel
    {
        [Required]
        public int ScheduleId { get; set; }
    }
}
