
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

        public ShowService(
            IShowRepository showRepository,
            IMovieRepository movieRepository,
            IScreenRepository screenRepository)
        {
            _showRepository = showRepository;
            _movieRepository = movieRepository;
            _screenRepository = screenRepository;
        }

        public async Task<ShowResponse> CreateAsync(CreateShowRequest request)
        {
            ValidateShowDetails(
                request.StartTime,
                request.EndTime,
                request.TicketPrice);

            var movie = await _movieRepository.GetByIdAsync(request.MovieId);

            if (movie == null)
            {
                throw new ArgumentException("Movie not found.");
            }

            var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            bool conflict = await _showRepository.HasScheduleConflictAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime);

            if (conflict)
            {
                throw new ArgumentException(
                    "Another show is already scheduled during this time on this screen.");
            }

            var show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TicketPrice = request.TicketPrice,
                Movie = movie,
                Screen = screen
            };

            await _showRepository.AddAsync(show);

            return MapToResponse(show);
        }

        public async Task<List<ShowResponse>> GetAllAsync()
        {
            var shows = await _showRepository.GetAllAsync();

            return MapToResponseList(shows);
        }

        public async Task<ShowResponse?> GetByIdAsync(int id)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                return null;
            }

            return MapToResponse(show);
        }

        public async Task<List<ShowResponse>> GetByMovieIdAsync(int movieId)
        {
            var shows = await _showRepository.GetByMovieIdAsync(movieId);

            return MapToResponseList(shows);
        }

        public async Task<List<ShowResponse>> GetByScreenIdAsync(int screenId)
        {
            var shows = await _showRepository.GetByScreenIdAsync(screenId);

            return MapToResponseList(shows);
        }

        public async Task<ShowResponse?> UpdateAsync(
            int id,
            UpdateShowRequest request)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
            {
                return null;
            }

            ValidateShowDetails(
                request.StartTime,
                request.EndTime,
                request.TicketPrice);

            var movie = await _movieRepository.GetByIdAsync(request.MovieId);

            if (movie == null)
            {
                throw new ArgumentException("Movie not found.");
            }

            var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException("Screen not found.");
            }

            bool conflict = await _showRepository.HasScheduleConflictAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime,
                id);

            if (conflict)
            {
                throw new ArgumentException(
                    "Another show is already scheduled during this time on this screen.");
            }

            show.MovieId = request.MovieId;
            show.ScreenId = request.ScreenId;
            show.StartTime = request.StartTime;
            show.EndTime = request.EndTime;
            show.TicketPrice = request.TicketPrice;
            show.Movie = movie;
            show.Screen = screen;

            await _showRepository.UpdateAsync(show);

            return MapToResponse(show);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _showRepository.DeleteAsync(id);
        }

        private void ValidateShowDetails(
            DateTime startTime,
            DateTime endTime,
            decimal ticketPrice)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException(
                    "End time must be later than start time.");
            }

            if (ticketPrice < 0)
            {
                throw new ArgumentException(
                    "Ticket price cannot be negative.");
            }
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

        private List<ShowResponse> MapToResponseList(List<Show> shows)
        {
            var responses = new List<ShowResponse>();

            foreach (var show in shows)
            {
                responses.Add(MapToResponse(show));
            }

            return responses;
        }
    }
}
