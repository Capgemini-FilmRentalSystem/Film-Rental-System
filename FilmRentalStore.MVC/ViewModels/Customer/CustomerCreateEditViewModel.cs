using FilmRentalStore.MVC.DTOs.Customers;
using FilmRentalStore.MVC.DTOs.Address;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Customer
{
    public class CustomerCreateEditViewModel
    {
        public int CustomerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int StoreId { get; set; }
        public bool IsActive { get; set; } = true;
        public AddressDto Address { get; set; } = new();
        public List<SelectListItem> Stores { get; set; } = new();
        public bool IsEditMode => CustomerId > 0;

        public CustomerRequestDto ToRequestDto()
        {
            return new CustomerRequestDto
            {
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                StoreId = StoreId,
                Password = string.IsNullOrWhiteSpace(Password) ? null : Password,
                Address = Address.ToRequestDto()
            };
        }

        public static CustomerCreateEditViewModel FromResponseDto(CustomerResponseDto dto)
        {
            return new CustomerCreateEditViewModel
            {
                CustomerId = dto.CustomerId,
                Username = dto.Username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                StoreId = dto.StoreId,
                IsActive = dto.IsActive,
                Address = AddressDto.FromResponseDto(dto.Address)
            };
        }
    }
}
