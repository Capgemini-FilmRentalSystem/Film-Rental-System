using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.DTOs;

namespace FilmRentalStore.API.DTOs.Staff
{
    public class StaffResponseDto
    {
        public byte StaffId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AddressId { get; set; }
        public int StoreId { get; set; }
        public int RoleId { get; set; }
        public string? Email { get; set; }
        public string Username { get; set; } = null!;
        public bool Active { get; set; }
        public DateTime LastUpdate { get; set; }
        public RoleSummaryDto Role { get; set; } = null!;
        public StoreSummaryDto Store { get; set; } = null!;
        public AddressResponseDto Address { get; set; } = null!;
    }
}
