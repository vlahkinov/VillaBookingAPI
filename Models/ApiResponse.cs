using System.Net;

namespace VillaBookingAPI.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> Errors { get; set; } = new();
        public object? Result { get; set; }
    }
}