using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models
{
    public class Theatre
    {
        public int TheatreId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
        public ICollection<Screen> Screens { get; set; } = new List<Screen>();
    }
}
