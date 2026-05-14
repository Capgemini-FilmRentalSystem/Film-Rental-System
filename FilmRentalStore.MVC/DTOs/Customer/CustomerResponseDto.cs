using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.DTOs;

namespace FilmRentalStore.MVC.DTOs.Customers
{
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public RoleSummaryDto? Role { get; set; }
        public AddressResponseDto? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
