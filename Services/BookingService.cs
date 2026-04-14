using Microsoft.EntityFrameworkCore;
using VillaBookingAPI.Data;
using VillaBookingAPI.Models;
using VillaBookingAPI.Models.Dto;

namespace VillaBookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _db;

        public BookingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _db.Bookings
                .OrderBy(b => b.StartDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _db.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> CreateBookingAsync(BookingCreateDto dto)
        {
            var booking = dto.ToEntity();

            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            return booking;
        }

        public async Task<Booking?> UpdateBookingAsync(int id, BookingUpdateDto dto)
        {
            var existing = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id);

            if (existing == null)
                return null;

            existing.UpdateFrom(dto);

            await _db.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return false;

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Checks whether any existing booking for the given house overlaps
        /// with the requested date range. Optionally excludes a booking by Id
        /// (used during updates so the booking being edited doesn't conflict with itself).
        ///
        /// Overlap condition: two intervals [A_start, A_end) and [B_start, B_end) overlap
        /// when A_start < B_end AND B_start < A_end.
        /// </summary>
        public async Task<bool> HasOverlappingBookingAsync(
            int houseId, DateTime startDate, DateTime endDate, int? excludeBookingId = null)
        {
            var query = _db.Bookings
                .Where(b => b.HouseId == houseId)
                .Where(b => b.StartDate < endDate && startDate < b.EndDate);

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
