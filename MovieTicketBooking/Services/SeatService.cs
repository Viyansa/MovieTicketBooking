using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interface;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IScreenRepository _screenRepository;

        public SeatService(
            ISeatRepository seatRepository,
            IScreenRepository screenRepository)
        {
            _seatRepository = seatRepository;
            _screenRepository = screenRepository;
        }

        public async Task<SeatResponse> CreateAsync(CreateSeatRequest request)
        {
            var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            string rowName = request.RowName.Trim().ToUpperInvariant();

            bool exists = await _seatRepository.ExistsAsync(
                request.ScreenId, rowName, request.SeatNumber);

            if (exists)
            {
                throw new ArgumentException("This seat already exists in the screen.");
            }

            var seat = new Seat
            {
                ScreenId = request.ScreenId,
                RowName = rowName,
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType.Trim()
            };

            var createdSeat = await _seatRepository.AddAsync(seat);

            return MapToResponse(createdSeat);
        }

        public async Task<List<SeatResponse>> BulkCreateAsync(
            BulkCreateSeatsRequest request)
        {
            var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            var seats = new List<Seat>();

            for (int i = 0; i < request.Seats.Count; i++)
            {
                var item = request.Seats[i];

                string rowName = item.RowName.Trim().ToUpperInvariant();

                for (int j = 0; j < i; j++)
                {
                    string previousRow = request.Seats[j].RowName
                        .Trim().ToUpperInvariant();

                    if (previousRow == rowName &&
                        request.Seats[j].SeatNumber == item.SeatNumber)
                    {
                        throw new ArgumentException("Duplicate seat found in the request.");
                    }
                }

                bool exists = await _seatRepository.ExistsAsync(
                    request.ScreenId, rowName, item.SeatNumber);

                if (exists)
                {
                    throw new ArgumentException(
                        $"Seat {rowName}{item.SeatNumber} already exists.");
                }

                seats.Add(new Seat
                {
                    ScreenId = request.ScreenId,
                    RowName = rowName,
                    SeatNumber = item.SeatNumber,
                    SeatType = item.SeatType.Trim()
                });
            }

            var createdSeats = await _seatRepository.AddRangeAsync(seats);

            return createdSeats.Select(MapToResponse).ToList();
        }

        public async Task<List<SeatResponse>> GetByScreenIdAsync(int screenId)
        {
            var screen = await _screenRepository.GetByIdAsync(screenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            var seats = await _seatRepository.GetByScreenIdAsync(screenId);

            return seats.Select(MapToResponse).ToList();
        }

        public async Task<SeatResponse?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return null;
            }

            return MapToResponse(seat);
        }

        public async Task<SeatResponse?> UpdateAsync(
            int id,
            UpdateSeatRequest request)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return null;
            }

            var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            string rowName = request.RowName.Trim().ToUpperInvariant();

            bool exists = await _seatRepository.ExistsAsync(
                request.ScreenId,
                rowName,
                request.SeatNumber,
                id);

            if (exists)
            {
                throw new ArgumentException("This seat already exists in the screen.");
            }

            seat.ScreenId = request.ScreenId;
            seat.RowName = rowName;
            seat.SeatNumber = request.SeatNumber;
            seat.SeatType = request.SeatType.Trim();

            await _seatRepository.UpdateAsync(seat);

            return MapToResponse(seat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _seatRepository.DeleteAsync(id);
        }

        private SeatResponse MapToResponse(Seat seat)
        {
            return new SeatResponse
            {
                SeatId = seat.SeatId,
                ScreenId = seat.ScreenId,
                RowName = seat.RowName,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType
            };
        }
    }
}
