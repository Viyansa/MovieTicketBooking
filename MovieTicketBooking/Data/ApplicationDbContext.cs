using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Screen>()
                .HasOne(s => s.Theatre)
                .WithMany(t => t.Screens)
                .HasForeignKey(s => s.TheatreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Screen>()
                .HasIndex(s => new { s.TheatreId, s.ScreenName })
                .IsUnique();
            modelBuilder.Entity<Seat>()
    .HasOne(s => s.Screen)
    .WithMany(s => s.Seats)
    .HasForeignKey(s => s.ScreenId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.ScreenId, s.RowName, s.SeatNumber })
                .IsUnique();
            modelBuilder.Entity<Show>()
    .HasOne(s => s.Movie)
    .WithMany(m => m.Shows)
    .HasForeignKey(s => s.MovieId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Show>()
                .HasOne(s => s.Screen)
                .WithMany(sc => sc.Shows)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

