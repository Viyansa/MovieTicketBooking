
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }

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

            modelBuilder.Entity<ShowSeat>()
                .HasOne(ss => ss.Show)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShowSeat>()
                .HasOne(ss => ss.Seat)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShowSeat>()
                .HasIndex(ss => new { ss.ShowId, ss.SeatId })
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Show)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingReference)
                .IsUnique();

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Seat)
                .WithMany(s => s.BookingSeats)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Show>()
    .Property(s => s.TicketPrice)
    .HasPrecision(10, 2);
        }
    }
}
