using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interface
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateAsync(CreateBookingRequest request);

        Task<List<BookingResponse>> GetAllAsync();

        Task<BookingResponse?> GetByIdAsync(int id);

        Task<List<BookingResponse>> GetByShowIdAsync(int showId);

        Task<BookingResponse?> CancelAsync(int id);
    }
}
