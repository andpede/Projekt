using Microsoft.AspNetCore.Identity;

namespace SurfUpRedux.Models
{
    public class SurfUpUser : IdentityUser
    {
        public ICollection<Booking> Bookings { get; set; }
    }
}
