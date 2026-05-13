using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.DTOs;

namespace FilmRentalStore.API.DTOs.Customers
{
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string? Email { get; set; }
        public RoleSummaryDto Role { get; set; } = null!;
        public AddressResponseDto Address { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
