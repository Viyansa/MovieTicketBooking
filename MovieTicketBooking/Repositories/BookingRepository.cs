
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interface;

namespace MovieTicketBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateAsync(
            Booking booking, List<ShowSeat> showSeats)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                booking.BookingReference =
                    "TEMP-" + Guid.NewGuid().ToString("N");

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                booking.BookingReference = "MOV" + booking.BookingId;

                foreach (ShowSeat showSeat in showSeats)
                {
                    showSeat.Status = "Booked";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Show)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Show)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<List<Booking>> GetByShowIdAsync(int showId)
        {
            return await _context.Bookings
                .Include(b => b.Show)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.ShowId == showId)
                .ToListAsync();
        }

        public async Task CancelAsync(
            Booking booking, List<ShowSeat> showSeats)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                booking.Status = "Cancelled";

                foreach (ShowSeat showSeat in showSeats)
                {
                    showSeat.Status = "Available";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
