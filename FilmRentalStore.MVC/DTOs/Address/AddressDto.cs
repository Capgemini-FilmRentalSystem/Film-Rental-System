namespace FilmRentalStore.MVC.DTOs.Address
{
    public class AddressDto
    {
        public string AddressLine { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string District { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public AddressRequestDto ToRequestDto()
        {
            return new AddressRequestDto
            {
                AddressLine = AddressLine,
                Address2 = Address2,
                District = District,
                PostalCode = PostalCode,
                Phone = Phone,
                CityName = City,
                CountryName = Country
            };
        }

        public static AddressDto FromResponseDto(AddressResponseDto? dto)
        {
            if (dto == null)
            {
                return new AddressDto();
            }

            return new AddressDto
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
