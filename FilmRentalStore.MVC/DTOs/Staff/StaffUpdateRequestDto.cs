using FilmRentalStore.MVC.DTOs.Address;

namespace FilmRentalStore.MVC.DTOs.Staff
{
    public class StaffUpdateRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AddressId { get; set; }
        public string? Email { get; set; }
        public bool Active { get; set; } = true;
        public int RoleId { get; set; }
        public AddressDto? Address { get; set; }
    }
}
