using System.ComponentModel.DataAnnotations;

namespace VillaBookingAPI.Models.Dto
{
    public class BookingCreateDto
    {
        [Required(ErrorMessage = "ClientName is required.")]
        [MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;

        [Required]
        [Range(1, 4, ErrorMessage = "GuestsCount must be between 1 and 4.")]
        public int GuestsCount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "HouseId must be 1 or 2.")]
        public int HouseId { get; set; }

        public bool IsDepositPaid { get; set; }

        [Required(ErrorMessage = "CreatedBy is required.")]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
