using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _repository;

        public TheatreService(ITheatreRepository repository)
        {
            _repository = repository;
        }

        public async Task<TheatreResponse> CreateAsync(CreateTheatreRequest request)
        {
            var theatre = new Theatre
            {
                Name = request.Name,
                Location = request.Location
            };

            var result = await _repository.AddAsync(theatre);

            return MapToResponse(result);
        }

        public async Task<List<TheatreResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var theatres = await _repository.GetAllAsync(pageNumber, pageSize);

            return theatres.Select(MapToResponse).ToList();
        }

        public async Task<TheatreResponse?> GetByIdAsync(int id)
        {
            var theatre = await _repository.GetByIdAsync(id);

            if (theatre == null)
            {
                return null;
            }

            return MapToResponse(theatre);
        }

        public async Task<TheatreResponse?> UpdateAsync(int id, UpdateTheatreRequest request)
        {
            var theatre = await _repository.GetByIdAsync(id);

            if (theatre == null)
            {
                return null;
            }

            theatre.Name = request.Name;
            theatre.Location = request.Location;

            await _repository.UpdateAsync(theatre);

            return MapToResponse(theatre);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private TheatreResponse MapToResponse(Theatre theatre)
        {
            return new TheatreResponse
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                Location = theatre.Location
            };
        }
    }
}