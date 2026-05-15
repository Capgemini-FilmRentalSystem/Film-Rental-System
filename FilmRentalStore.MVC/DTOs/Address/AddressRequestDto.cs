namespace FilmRentalStore.MVC.DTOs.Address
{
    public class AddressRequestDto
    {
        public string? AddressLine { get; set; }
        public string? Address2 { get; set; }
        public string? District { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? CityName { get; set; }
        public string? CountryName { get; set; }
    }
}
