using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.DTOs.Store;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Store
{
    public class StoreFormViewModel
    {
        public int StoreId { get; set; }
        public byte ManagerStaffId { get; set; }
        public int AddressId { get; set; }
        public AddressDto Address { get; set; } = new();
        public List<SelectListItem> Managers { get; set; } = new();
        public bool IsEditMode => StoreId > 0;

        public StoreRequestDto ToRequestDto()
        {
            return new StoreRequestDto
            {
                ManagerStaffId = ManagerStaffId,
                AddressId = AddressId,
                Address = Address
            };
        }

        public static StoreFormViewModel FromResponseDto(StoreResponseDto dto)
        {
            return new StoreFormViewModel
            {
                StoreId = dto.StoreId,
                ManagerStaffId = dto.ManagerStaffId,
                AddressId = dto.AddressId,
                Address = AddressDto.FromResponseDto(dto.Address)
            };
        }
    }
}
