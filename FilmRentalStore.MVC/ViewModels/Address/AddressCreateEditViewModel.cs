using FilmRentalStore.MVC.DTOs.Address;

namespace FilmRentalStore.MVC.ViewModels.Address
{
    public class AddressCreateEditViewModel
    {
        public int AddressId { get; set; }
        public string AddressLine { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string District { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsEditMode => AddressId > 0;

        public AddressRequestDto ToRequestDto()
        {
            return new AddressRequestDto
            {
                Address1 = AddressLine,
                Address2 = Address2,
                District = District,
                PostalCode = PostalCode,
                Phone = Phone,
                City = City,
                Country = Country
            };
        }

        public static AddressCreateEditViewModel FromResponseDto(AddressResponseDto dto)
        {
            return new AddressCreateEditViewModel
            {
                AddressLine = dto.AddressLine,
                Address2 = dto.Address2,
                District = dto.District,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                City = dto.City,
                Country = dto.Country
            };
        }
    }
}
