using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interface;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interface;

namespace MovieTicketBooking.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _screenRepository;
        private readonly ITheatreRepository _theatreRepository;

        public ScreenService(
            IScreenRepository screenRepository,
            ITheatreRepository theatreRepository)
        {
            _screenRepository = screenRepository;
            _theatreRepository = theatreRepository;
        }

        public async Task<ScreenResponse> CreateAsync(CreateScreenRequest request)
        {
            var theatre = await _theatreRepository.GetByIdAsync(request.TheatreId);

            if (theatre == null)
            {
                throw new ArgumentException("Theatre not found.");
            }

            bool exists = await _screenRepository.ExistsByNameAsync(
                request.TheatreId, request.ScreenName.Trim());

            if (exists)
            {
                throw new ArgumentException("Screen name already exists in this theatre.");
            }

            var screen = new Screen
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName.Trim()
            };

            var createdScreen = await _screenRepository.AddAsync(screen);

            return MapToResponse(createdScreen);
        }

        public async Task<List<ScreenResponse>> GetAllAsync(int? theatreId)
        {
            var screens = await _screenRepository.GetAllAsync(theatreId);

            return screens.Select(MapToResponse).ToList();
        }

        public async Task<ScreenResponse?> GetByIdAsync(int id)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
            {
                return null;
            }

            return MapToResponse(screen);
        }

        public async Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId)
        {
            var theatre = await _theatreRepository.GetByIdAsync(theatreId);

            if (theatre == null)
            {
                throw new ArgumentException("Theatre not found.");
            }

            var screens = await _screenRepository.GetByTheatreIdAsync(theatreId);

            return screens.Select(MapToResponse).ToList();
        }

        public async Task<ScreenResponse?> UpdateAsync(
            int id,
            UpdateScreenRequest request)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
            {
                return null;
            }

            var theatre = await _theatreRepository.GetByIdAsync(request.TheatreId);

            if (theatre == null)
            {
                throw new ArgumentException("Theatre not found.");
            }

            bool exists = await _screenRepository.ExistsByNameAsync(
                request.TheatreId,
                request.ScreenName.Trim(),
                id);

            if (exists)
            {
                throw new ArgumentException("Screen name already exists in this theatre.");
            }

            screen.TheatreId = request.TheatreId;
            screen.ScreenName = request.ScreenName.Trim();

            await _screenRepository.UpdateAsync(screen);

            return MapToResponse(screen);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _screenRepository.DeleteAsync(id);
        }

        private ScreenResponse MapToResponse(Screen screen)
        {
            return new ScreenResponse
            {
                ScreenId = screen.ScreenId,
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName
            };
        }
    }
}
