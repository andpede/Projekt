using Microsoft.AspNetCore.Identity;

namespace SurfUpRedux.Models
{
    public class SurfUpUser : IdentityUser
    {
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
