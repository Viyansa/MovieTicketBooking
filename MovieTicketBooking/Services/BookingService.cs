
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interface;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IShowRepository _showRepository;
        private readonly IShowSeatRepository _showSeatRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IShowRepository showRepository,
            IShowSeatRepository showSeatRepository)
        {
            _bookingRepository = bookingRepository;
            _showRepository = showRepository;
            _showSeatRepository = showSeatRepository;
        }

        public async Task<BookingResponse> CreateAsync(
            CreateBookingRequest request)
        {
            if (request.SeatIds == null || request.SeatIds.Count == 0)
            {
                throw new ArgumentException("Select at least one seat.");
            }

            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
            {
                throw new ArgumentException("Duplicate seat IDs are not allowed.");
            }

            Show? show = await _showRepository.GetByIdAsync(request.ShowId);

            if (show == null)
            {
                throw new KeyNotFoundException("Show not found.");
            }

            List<ShowSeat> allShowSeats =
                await _showSeatRepository.GetByShowIdAsync(request.ShowId);

            List<ShowSeat> selectedShowSeats = new List<ShowSeat>();
            List<BookingSeat> bookingSeats = new List<BookingSeat>();

            foreach (int seatId in request.SeatIds)
            {
                ShowSeat? showSeat = allShowSeats
                    .FirstOrDefault(s => s.SeatId == seatId);

                if (showSeat == null)
                {
                    throw new InvalidOperationException(
                        "Seat " + seatId + " does not belong to this show.");
                }

                if (showSeat.Status != "Available")
                {
                    throw new InvalidOperationException(
                        "Seat " + seatId + " is already booked.");
                }

                selectedShowSeats.Add(showSeat);

                bookingSeats.Add(new BookingSeat
                {
                    SeatId = seatId,
                    Price = show.TicketPrice,
                    Seat = showSeat.Seat
                });
            }

            decimal totalAmount =
                show.TicketPrice * request.SeatIds.Count;

            Booking booking = new Booking
            {
                ShowId = request.ShowId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                BookingDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Confirmed",
                BookingSeats = bookingSeats
            };

            Booking createdBooking =
                await _bookingRepository.CreateAsync(
                    booking, selectedShowSeats);

            return MapToResponse(createdBooking);
        }

        public async Task<List<BookingResponse>> GetAllAsync()
        {
            List<Booking> bookings =
                await _bookingRepository.GetAllAsync();

            List<BookingResponse> responses =
                new List<BookingResponse>();

            foreach (Booking booking in bookings)
            {
                responses.Add(MapToResponse(booking));
            }

            return responses;
        }

        public async Task<BookingResponse?> GetByIdAsync(int id)
        {
            Booking? booking =
                await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return null;
            }

            return MapToResponse(booking);
        }

        public async Task<List<BookingResponse>> GetByShowIdAsync(
            int showId)
        {
            List<Booking> bookings =
                await _bookingRepository.GetByShowIdAsync(showId);

            List<BookingResponse> responses =
                new List<BookingResponse>();

            foreach (Booking booking in bookings)
            {
                responses.Add(MapToResponse(booking));
            }

            return responses;
        }

        public async Task<BookingResponse?> CancelAsync(int id)
        {
            Booking? booking =
                await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return null;
            }

            if (booking.Status == "Cancelled")
            {
                throw new InvalidOperationException(
                    "This booking is already cancelled.");
            }

            List<ShowSeat> allShowSeats =
                await _showSeatRepository.GetByShowIdAsync(booking.ShowId);

            List<ShowSeat> selectedShowSeats =
                new List<ShowSeat>();

            foreach (BookingSeat bookingSeat in booking.BookingSeats)
            {
                ShowSeat? showSeat = allShowSeats
                    .FirstOrDefault(s => s.SeatId == bookingSeat.SeatId);

                if (showSeat == null || showSeat.Status != "Booked")
                {
                    throw new InvalidOperationException(
                        "The booking seat status is invalid.");
                }

                selectedShowSeats.Add(showSeat);
            }

            await _bookingRepository.CancelAsync(
                booking, selectedShowSeats);

            booking.Status = "Cancelled";

            return MapToResponse(booking);
        }

        private BookingResponse MapToResponse(Booking booking)
        {
            BookingResponse response = new BookingResponse
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingReference,
                ShowId = booking.ShowId,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };

            foreach (BookingSeat bookingSeat in booking.BookingSeats)
            {
                if (bookingSeat.Seat != null)
                {
                    string seatLabel =
                        bookingSeat.Seat.RowName +
                        bookingSeat.Seat.SeatNumber;

                    response.Seats.Add(seatLabel);
                }
            }

            return response;
        }
    }
}
