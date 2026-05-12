namespace FilmRentalStore.API.DTOs.Address
{
    public class AddressResponseDto
    {
        public string AddressLine { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string District { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
    }
}
