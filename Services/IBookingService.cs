using VillaBookingAPI.Models;
using VillaBookingAPI.Models.Dto;

namespace VillaBookingAPI.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> CreateBookingAsync(BookingCreateDto dto);
        Task<Booking?> UpdateBookingAsync(int id, BookingUpdateDto dto);
        Task<bool> DeleteBookingAsync(int id);
        Task<bool> HasOverlappingBookingAsync(int houseId, DateTime startDate, DateTime endDate, int? excludeBookingId = null);
    }
}
