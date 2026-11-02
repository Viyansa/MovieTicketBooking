
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interface;

namespace MovieTicketBooking.Repositories
{
    public class ShowSeatRepository : IShowSeatRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowSeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<ShowSeat> showSeats)
        {
            await _context.ShowSeats.AddRangeAsync(showSeats);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShowSeat>> GetByShowIdAsync(int showId)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId)
                .OrderBy(ss => ss.Seat!.RowName)
                .ThenBy(ss => ss.Seat!.SeatNumber)
                .ToListAsync();
        }
    }
}
