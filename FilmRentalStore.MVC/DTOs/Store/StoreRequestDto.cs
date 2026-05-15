using FilmRentalStore.MVC.DTOs.Address;

namespace FilmRentalStore.MVC.DTOs.Store
{
    public class StoreRequestDto
    {
        public byte ManagerStaffId { get; set; }
        public int AddressId { get; set; }
        public AddressDto? Address { get; set; }
    }
}
