using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaBookingAPI.Data;
using VillaBookingAPI.Models;

namespace VillaBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HousesController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// GET /api/houses/{houseId}/bookings?month=7&year=2025
        /// Връща всички резервации за дадена къща, филтрирани по месец/година.
        /// Полезно за MAUI приложението при визуализация на календар.
        /// </summary>
        [HttpGet("{houseId:int}/bookings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetBookingsByHouse(
            int houseId,
            [FromQuery] int? month,
            [FromQuery] int? year)
        {
            var response = new ApiResponse();

            try
            {
                if (houseId < 1 || houseId > 2)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("HouseId must be 1 or 2.");
                    return BadRequest(response);
                }

                var query = _db.Bookings
                    .Where(b => b.HouseId == houseId)
                    .AsNoTracking();

                // Филтриране по месец и година (ако са подадени)
                if (month.HasValue && year.HasValue)
                {
                    var periodStart = new DateTime(year.Value, month.Value, 1);
                    var periodEnd = periodStart.AddMonths(1);

                    // Включваме резервации, които припокриват избрания месец
                    query = query.Where(b => b.StartDate < periodEnd && b.EndDate > periodStart);
                }

                var bookings = await query
                    .OrderBy(b => b.StartDate)
                    .ToListAsync();

                response.StatusCode = HttpStatusCode.OK;
                response.Result = bookings;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add($"An error occurred: {ex.Message}");

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// GET /api/houses/{houseId}/availability?startDate=2025-07-10&endDate=2025-07-15
        /// Проверява дали къщата е свободна за даден период.
        /// Полезно за MAUI приложението при създаване на нова резервация.
        /// </summary>
        [HttpGet("{houseId:int}/availability")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CheckAvailability(
            int houseId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var response = new ApiResponse();

            try
            {
                if (houseId < 1 || houseId > 2)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("HouseId must be 1 or 2.");
                    return BadRequest(response);
                }

                if (startDate >= endDate)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("StartDate must be before EndDate.");
                    return BadRequest(response);
                }

                var hasConflict = await _db.Bookings
                    .Where(b => b.HouseId == houseId)
                    .Where(b => b.StartDate < endDate && startDate < b.EndDate)
                    .AnyAsync();

                response.StatusCode = HttpStatusCode.OK;
                response.Result = new
                {
                    HouseId = houseId,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsAvailable = !hasConflict
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add($"An error occurred: {ex.Message}");

                return StatusCode(500, response);
            }
        }
    }
}
