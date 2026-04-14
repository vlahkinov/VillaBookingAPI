using System.Net;
using Microsoft.AspNetCore.Mvc;
using VillaBookingAPI.Models;
using VillaBookingAPI.Models.Dto;
using VillaBookingAPI.Services;

namespace VillaBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/bookings
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetAllBookings()
        {
            var response = new ApiResponse();

            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();

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

        // GET: api/bookings/{id}
        [HttpGet("{id:int}", Name = "GetBookingById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetBookingById(int id)
        {
            var response = new ApiResponse();

            try
            {
                if (id <= 0)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("Id must be a positive integer.");
                    return BadRequest(response);
                }

                var booking = await _bookingService.GetBookingByIdAsync(id);

                if (booking == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Errors.Add($"Booking with Id = {id} was not found.");
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result = booking;

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

        // POST: api/bookings
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CreateBooking([FromBody] BookingCreateDto dto)
        {
            var response = new ApiResponse();

            try
            {
                // ModelState validation (data annotations on DTO)
                if (!ModelState.IsValid)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(response);
                }

                // Business rule: StartDate must be before EndDate
                if (dto.StartDate >= dto.EndDate)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("StartDate must be before EndDate.");
                    return BadRequest(response);
                }

                // Business rule: No overlapping bookings for the same house
                bool hasOverlap = await _bookingService.HasOverlappingBookingAsync(
                    dto.HouseId, dto.StartDate, dto.EndDate);

                if (hasOverlap)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add(
                        $"House {dto.HouseId} already has a booking that overlaps " +
                        $"with the requested period {dto.StartDate:yyyy-MM-dd} – {dto.EndDate:yyyy-MM-dd}.");
                    return BadRequest(response);
                }

                var created = await _bookingService.CreateBookingAsync(dto);

                response.StatusCode = HttpStatusCode.Created;
                response.Result = created;

                return CreatedAtRoute("GetBookingById", new { id = created.Id }, response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add($"An error occurred: {ex.Message}");

                return StatusCode(500, response);
            }
        }

        // PUT: api/bookings/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> UpdateBooking(int id, [FromBody] BookingUpdateDto dto)
        {
            var response = new ApiResponse();

            try
            {
                if (!ModelState.IsValid)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(response);
                }

                if (id <= 0 || id != dto.Id)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("Route id must match the body Id and be positive.");
                    return BadRequest(response);
                }

                // Business rule: StartDate must be before EndDate
                if (dto.StartDate >= dto.EndDate)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("StartDate must be before EndDate.");
                    return BadRequest(response);
                }

                // Business rule: No overlapping bookings (exclude current booking)
                bool hasOverlap = await _bookingService.HasOverlappingBookingAsync(
                    dto.HouseId, dto.StartDate, dto.EndDate, excludeBookingId: id);

                if (hasOverlap)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add(
                        $"House {dto.HouseId} already has a booking that overlaps " +
                        $"with the requested period {dto.StartDate:yyyy-MM-dd} – {dto.EndDate:yyyy-MM-dd}.");
                    return BadRequest(response);
                }

                var updated = await _bookingService.UpdateBookingAsync(id, dto);

                if (updated == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Errors.Add($"Booking with Id = {id} was not found.");
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result = updated;

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

        // DELETE: api/bookings/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> DeleteBooking(int id)
        {
            var response = new ApiResponse();

            try
            {
                if (id <= 0)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("Id must be a positive integer.");
                    return BadRequest(response);
                }

                bool deleted = await _bookingService.DeleteBookingAsync(id);

                if (!deleted)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Errors.Add($"Booking with Id = {id} was not found.");
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Result = $"Booking with Id = {id} has been deleted.";

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
