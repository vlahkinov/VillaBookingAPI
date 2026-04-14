using VillaBookingAPI.Models;
using VillaBookingAPI.Models.Dto;

namespace VillaBookingAPI.Services
{
    /// <summary>
    /// Extension методи за преобразуване между DTO обекти и Entity модели.
    /// Алтернатива на AutoMapper за по-прост проект.
    /// </summary>
    public static class MappingExtensions
    {
        public static Booking ToEntity(this BookingCreateDto dto)
        {
            return new Booking
            {
                ClientName = dto.ClientName,
                GuestsCount = dto.GuestsCount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                HouseId = dto.HouseId,
                IsDepositPaid = dto.IsDepositPaid,
                CreatedBy = dto.CreatedBy
            };
        }

        public static void UpdateFrom(this Booking entity, BookingUpdateDto dto)
        {
            entity.ClientName = dto.ClientName;
            entity.GuestsCount = dto.GuestsCount;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.HouseId = dto.HouseId;
            entity.IsDepositPaid = dto.IsDepositPaid;
            entity.CreatedBy = dto.CreatedBy;
        }

        public static BookingUpdateDto ToUpdateDto(this Booking entity)
        {
            return new BookingUpdateDto
            {
                Id = entity.Id,
                ClientName = entity.ClientName,
                GuestsCount = entity.GuestsCount,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                HouseId = entity.HouseId,
                IsDepositPaid = entity.IsDepositPaid,
                CreatedBy = entity.CreatedBy
            };
        }
    }
}
