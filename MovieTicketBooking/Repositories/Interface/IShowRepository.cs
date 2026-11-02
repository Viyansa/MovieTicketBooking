using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interface
{
    public interface IShowRepository
    {
        Task<Show> AddAsync(Show show);

        Task<List<Show>> GetAllAsync();

        Task<Show?> GetByIdAsync(int id);

        Task<List<Show>> GetByMovieIdAsync(int movieId);

        Task<List<Show>> GetByScreenIdAsync(int screenId);

        Task<bool> HasScheduleConflictAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null);

        Task UpdateAsync(Show show);

        Task<bool> DeleteAsync(int id);
    }
}