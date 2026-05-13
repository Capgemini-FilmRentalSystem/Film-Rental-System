using FilmRentalStore.API.DTOs.Address;

namespace FilmRentalStore.API.DTOs.Customers
{
    /// <summary>
    /// Unified DTO for all customer request operations (Create, Update, Register)
    /// </summary>
    public class CustomerRequestDto
    {
        public int StoreId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Username { get; set; }  // Required for create/register, optional for update
        public string? Password { get; set; }  // Required for create/register, optional for update
        public AddressRequestDto? Address { get; set; }
    }
}
