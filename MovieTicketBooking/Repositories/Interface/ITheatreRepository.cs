using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ITheatreRepository
    {
        Task<Theatre> AddAsync(Theatre theatre);

        Task<List<Theatre>> GetAllAsync(int pageNumber, int pageSize);

        Task<Theatre?> GetByIdAsync(int id);

        Task UpdateAsync(Theatre theatre);

        Task<bool> DeleteAsync(int id);
    }
}