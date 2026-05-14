using FilmRentalStore.MVC.DTOs.Address;

namespace FilmRentalStore.MVC.ViewModels.Address
{
    public class AddressIndexViewModel
    {
        public List<AddressResponseDto> Addresses { get; set; } = new List<AddressResponseDto>();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalAddresses { get; set; }
    }
}
