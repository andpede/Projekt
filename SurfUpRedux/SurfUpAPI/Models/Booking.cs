using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SurfUpReduxAPI.Models
{
    public class Booking
    {

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        
        public int BoardId { get; set; }

        
        public Board? Board { get; set; }

        
        public string UserId { get; set; }

        public Booking(DateTime _startDate, DateTime _endDate, int _boardId, string _userId)
        {
            Id = 0;
            StartDate = _startDate;
            EndDate = _endDate;
            BoardId = _boardId;
            UserId = _userId;
        }

    }
}
