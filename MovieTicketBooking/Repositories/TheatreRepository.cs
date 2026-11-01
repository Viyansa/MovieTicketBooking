using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class TheatreRepository : ITheatreRepository
    {
        private readonly ApplicationDbContext _context;

        public TheatreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Theatre> AddAsync(Theatre theatre)
        {
            _context.Theatres.Add(theatre);

            await _context.SaveChangesAsync();

            return theatre;
        }

        public async Task<List<Theatre>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Theatres
                .OrderBy(t => t.TheatreId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Theatre?> GetByIdAsync(int id)
        {
            return await _context.Theatres.FindAsync(id);
        }

        public async Task UpdateAsync(Theatre theatre)
        {
            _context.Theatres.Update(theatre);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var theatre = await _context.Theatres.FindAsync(id);

            if (theatre == null)
            {
                return false;
            }

            _context.Theatres.Remove(theatre);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
