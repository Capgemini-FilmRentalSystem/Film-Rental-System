namespace FilmRentalStore.API.DTOs.Customers
{
    public class CustomerCreateDto
    {
        public int StoreId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int? AddressId { get; set; }  // Existing address ID
        
        // Optional: Create new address inline
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? District { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? CityName { get; set; }  // City name (auto-create if needed)
        public string? CountryName { get; set; }  // Country name (auto-create if needed)
    }
}