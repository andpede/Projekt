using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SurfUpRedux.Models.ViewModel
{
    public class BookingHelper
    {

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int BoardId { get; set; }

        public string UserId { get; set; }

    }
}
