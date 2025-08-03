using GYM_APP.Models;

namespace GYM_APP.ViewModels
{
    public class QrGeneratorViewModel
    {
        public List<Booking> Bookings { get; set; } = new();
        public int UserId { get; set; }
    }
}
