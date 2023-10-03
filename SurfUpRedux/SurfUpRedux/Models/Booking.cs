using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SurfUpRedux.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        public string UserId { get; set; }

        public virtual SurfUpUser User { get; set; }
    }
}
