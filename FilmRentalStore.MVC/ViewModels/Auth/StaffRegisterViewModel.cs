using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Auth
{
    public class StaffRegisterViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public int RoleId { get; set; } = 3;
        public AddressDto Address { get; set; } = new();
        public List<SelectListItem> Stores { get; set; } = new();
        public List<SelectListItem> Roles { get; set; } = new();

        public RegisterRequestDto ToRequestDto()
        {
            return new RegisterRequestDto
            {
                Username = Username,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                StoreId = StoreId,
                RoleId = RoleId,
                Address = Address
            };
        }
    }
}
