using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interface
{
    public interface ISeatRepository
    {
        Task<Seat> AddAsync(Seat seat);
        Task<List<Seat>> AddRangeAsync(List<Seat> seats);
        Task<List<Seat>> GetByScreenIdAsync(int screenId);
        Task<Seat?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int screenId, string rowName, int seatNumber, int? excludeSeatId = null);
        Task UpdateAsync(Seat seat);
        Task<bool> DeleteAsync(int id);
    }
}