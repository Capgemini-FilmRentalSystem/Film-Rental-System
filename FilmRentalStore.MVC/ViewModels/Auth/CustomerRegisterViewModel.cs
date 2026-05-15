using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.DTOs.Customers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Auth
{
    public class CustomerRegisterViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public AddressDto Address { get; set; } = new();
        public List<SelectListItem> Stores { get; set; } = new();

        public CustomerRequestDto ToRequestDto()
        {
            return new CustomerRequestDto
            {
                Username = Username,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                StoreId = StoreId,
                Address = Address.ToRequestDto()
            };
        }
    }
}
