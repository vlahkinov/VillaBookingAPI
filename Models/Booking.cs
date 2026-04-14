using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaBookingAPI.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
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

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
