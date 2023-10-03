using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfUpRedux.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking End Date")]
        public DateTime EndDate { get; set; }

        [ForeignKey("Board")]
        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual SurfUpUser User { get; set; }
    }
}
