using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interface
{
    public interface ISeatService
    {
        Task<SeatResponse> CreateAsync(CreateSeatRequest request);
        Task<List<SeatResponse>> BulkCreateAsync(BulkCreateSeatsRequest request);
        Task<List<SeatResponse>> GetByScreenIdAsync(int screenId);
        Task<SeatResponse?> GetByIdAsync(int id);
        Task<SeatResponse?> UpdateAsync(int id, UpdateSeatRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
