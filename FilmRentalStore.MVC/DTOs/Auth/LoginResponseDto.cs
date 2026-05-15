using System.Text.Json.Serialization;

namespace FilmRentalStore.MVC.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token     { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public byte? StaffId     { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CustomerId     { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? StoreId     { get; set; }
        public string Username  { get; set; } = null!;
        public string Role      { get; set; } = null!;
    }
}
