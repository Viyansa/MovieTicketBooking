
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interface;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreenRepository _screenRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IShowSeatRepository _showSeatRepository;

        public ShowService(
            IShowRepository showRepository,
            IMovieRepository movieRepository,
            IScreenRepository screenRepository,
            ISeatRepository seatRepository,
            IShowSeatRepository showSeatRepository)
        {
            _showRepository = showRepository;
            _movieRepository = movieRepository;
            _screenRepository = screenRepository;
            _seatRepository = seatRepository;
            _showSeatRepository = showSeatRepository;
        }

        public async Task<ShowResponse> CreateAsync(CreateShowRequest request)
        {
            if (request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("End time must be greater than start time.");
            }

            if (request.TicketPrice < 0)
            {
                throw new ArgumentException("Ticket price cannot be negative.");
            }

            Movie? movie = await _movieRepository.GetByIdAsync(request.MovieId);

            if (movie == null)
            {
                throw new ArgumentException("Movie not found.");
            }

            Screen? screen = await _screenRepository.GetByIdAsync(request.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            bool hasConflict = await _showRepository.HasScheduleConflictAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime,
                null);

            if (hasConflict)
            {
                throw new ArgumentException("Another show is already scheduled during this time.");
            }

            Show show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TicketPrice = request.TicketPrice
            };

            Show createdShow = await _showRepository.AddAsync(show);

            List<Seat> seats =
                await _seatRepository.GetByScreenIdAsync(createdShow.ScreenId);

            List<ShowSeat> showSeats = new List<ShowSeat>();

            foreach (Seat seat in seats)
            {
                ShowSeat showSeat = new ShowSeat
                {
                    ShowId = createdShow.ShowId,
                    SeatId = seat.SeatId,
                    Status = "Available"
                };

                showSeats.Add(showSeat);
            }

            if (showSeats.Count > 0)
            {
                await _showSeatRepository.AddRangeAsync(showSeats);
            }

            return MapToResponse(createdShow);
        }

        public async Task<List<ShowResponse>> GetAllAsync()
        {
            List<Show> shows = await _showRepository.GetAllAsync();

            List<ShowResponse> response = new List<ShowResponse>();

            foreach (Show show in shows)
            {
                response.Add(MapToResponse(show));
            }

            return response;
        }

        public async Task<ShowResponse?> GetByIdAsync(int id)
        {
            Show? show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                return null;
            }

            return MapToResponse(show);
        }

        public async Task<List<ShowResponse>> GetByMovieIdAsync(int movieId)
        {
            List<Show> shows =
                await _showRepository.GetByMovieIdAsync(movieId);

            List<ShowResponse> response = new List<ShowResponse>();

            foreach (Show show in shows)
            {
                response.Add(MapToResponse(show));
            }

            return response;
        }

        public async Task<List<ShowResponse>> GetByScreenIdAsync(int screenId)
        {
            List<Show> shows =
                await _showRepository.GetByScreenIdAsync(screenId);

            List<ShowResponse> response = new List<ShowResponse>();

            foreach (Show show in shows)
            {
                response.Add(MapToResponse(show));
            }

            return response;
        }

        public async Task<ShowResponse?> UpdateAsync(
            int id,
            UpdateShowRequest request)
        {
            Show? show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                return null;
            }

            if (request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("End time must be greater than start time.");
            }

            if (request.TicketPrice < 0)
            {
                throw new ArgumentException("Ticket price cannot be negative.");
            }

            Movie? movie =
                await _movieRepository.GetByIdAsync(request.MovieId);

            if (movie == null)
            {
                throw new ArgumentException("Movie not found.");
            }

            Screen? screen =
                await _screenRepository.GetByIdAsync(request.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            bool hasConflict = await _showRepository.HasScheduleConflictAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime,
                id);

            if (hasConflict)
            {
                throw new ArgumentException("Another show is already scheduled during this time.");
            }

            show.MovieId = request.MovieId;
            show.ScreenId = request.ScreenId;
            show.StartTime = request.StartTime;
            show.EndTime = request.EndTime;
            show.TicketPrice = request.TicketPrice;

            await _showRepository.UpdateAsync(show);

            return MapToResponse(show);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Show? show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                return false;
            }

            await _showRepository.DeleteAsync(id);

            return true;
        }

        public async Task<List<ShowSeatResponse>> GetSeatsByShowIdAsync(int showId)
        {
            Show? show = await _showRepository.GetByIdAsync(showId);

            if (show == null)
            {
                throw new ArgumentException("Show not found.");
            }

            List<ShowSeat> showSeats =
                await _showSeatRepository.GetByShowIdAsync(showId);

            List<ShowSeatResponse> response =
                new List<ShowSeatResponse>();

            foreach (ShowSeat showSeat in showSeats)
            {
                if (showSeat.Seat != null)
                {
                    ShowSeatResponse seatResponse = new ShowSeatResponse
                    {
                        ShowSeatId = showSeat.ShowSeatId,
                        ShowId = showSeat.ShowId,
                        SeatId = showSeat.SeatId,
                        RowName = showSeat.Seat.RowName,
                        SeatNumber = showSeat.Seat.SeatNumber,
                        SeatType = showSeat.Seat.SeatType,
                        Status = showSeat.Status
                    };

                    response.Add(seatResponse);
                }
            }

            return response;
        }

        private ShowResponse MapToResponse(Show show)
        {
            return new ShowResponse
            {
                ShowId = show.ShowId,
                MovieId = show.MovieId,
                MovieTitle = show.Movie?.Title ?? string.Empty,
                ScreenId = show.ScreenId,
                ScreenName = show.Screen?.ScreenName ?? string.Empty,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                TicketPrice = show.TicketPrice
            };
        }
    }
}
