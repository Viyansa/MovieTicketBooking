using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class ShowSeat
    {
        public int ShowSeatId { get; set; }

        [Required]
        public int ShowId { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Available";

        public Show? Show { get; set; }

        public Seat? Seat { get; set; }
    }
}
