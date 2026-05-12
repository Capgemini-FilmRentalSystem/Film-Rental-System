using FilmRentalStore.API.DTOs.Address;

namespace FilmRentalStore.API.DTOs.Staff
{
    public class StaffWithAddressDto
    {
        public byte StaffId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string Username { get; set; } = null!;
        public string RoleTitle { get; set; } = null!;
        public int StoreId { get; set; }
        public bool Active { get; set; }
        public DateTime LastUpdate { get; set; }
        public AddressDto Address { get; set; } = null!;
    }
}
