using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SurfUpReduxAPI.Models
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

        
        public Board Board { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

    }
}
