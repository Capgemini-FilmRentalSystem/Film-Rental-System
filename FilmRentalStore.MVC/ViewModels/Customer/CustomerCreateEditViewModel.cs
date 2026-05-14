using FilmRentalStore.MVC.DTOs.Customers;

namespace FilmRentalStore.MVC.ViewModels.Customer
{
    public class CustomerCreateEditViewModel
    {
        public int CustomerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public bool IsActive { get; set; } = true;
        public List<StoreSelectItem> Stores { get; set; } = new List<StoreSelectItem>();
        public bool IsEditMode => CustomerId > 0;

        public CustomerRequestDto ToRequestDto()
        {
            return new CustomerRequestDto
            {
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                StoreId = StoreId
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
                IsActive = dto.IsActive
            };
        }
    }

    public class StoreSelectItem
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
    }
}
