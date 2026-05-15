using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.DTOs.Staff;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Staff
{
    public class StaffFormViewModel
    {
        public byte StaffId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public int RoleId { get; set; }
        public int AddressId { get; set; }
        public bool Active { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public AddressDto Address { get; set; } = new();
        public List<SelectListItem> Stores { get; set; } = new();
        public List<SelectListItem> Roles { get; set; } = new();
        public bool IsEditMode => StaffId > 0;

        public StaffCreateRequestDto ToCreateDto()
        {
            return new StaffCreateRequestDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                StoreId = StoreId,
                RoleId = RoleId,
                AddressId = AddressId,
                Active = Active,
                Username = Username,
                Password = Password,
                Address = Address
            };
        }

        public StaffUpdateRequestDto ToUpdateDto()
        {
            return new StaffUpdateRequestDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                RoleId = RoleId,
                AddressId = AddressId,
                Active = Active,
                Address = Address
            };
        }

        public static StaffFormViewModel FromResponseDto(StaffResponseDto dto)
        {
            return new StaffFormViewModel
            {
                StaffId = dto.StaffId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                StoreId = dto.StoreId,
                RoleId = dto.RoleId,
                AddressId = dto.AddressId,
                Active = dto.Active,
                Username = dto.Username,
                Address = AddressDto.FromResponseDto(dto.Address)
            };
        }
    }
}
