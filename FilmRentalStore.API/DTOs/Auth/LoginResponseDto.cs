namespace FilmRentalStore.API.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token     { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public byte StaffId     { get; set; }
        public string Username  { get; set; } = null!;
        public string Role      { get; set; } = null!;
    }
}