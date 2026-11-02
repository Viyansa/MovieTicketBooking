using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interface
{
    public interface IShowSeatRepository
    {
        Task AddRangeAsync(List<ShowSeat> showSeats);

        Task<List<ShowSeat>> GetByShowIdAsync(int showId);
    }
    
    }