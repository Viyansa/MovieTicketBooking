
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interface
{
    public interface IBookingRepository
    {
        Task<Booking> CreateAsync(Booking booking, List<ShowSeat> showSeats);

        Task<List<Booking>> GetAllAsync();

        Task<Booking?> GetByIdAsync(int id);

        Task<List<Booking>> GetByShowIdAsync(int showId);

        Task CancelAsync(Booking booking, List<ShowSeat> showSeats);
    }
}
