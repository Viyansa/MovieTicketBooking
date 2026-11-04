using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Services.Interface
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateAsync(
            CreateBookingRequest request,
            string userId);

        Task<List<BookingResponse>> GetAllAsync();

        Task<BookingResponse?> GetByIdAsync(int id);

        Task<Booking?> GetBookingEntityByIdAsync(int id);

        Task<List<BookingResponse>> GetByShowIdAsync(int showId);

        Task<BookingResponse?> CancelAsync(int id);
    }
}