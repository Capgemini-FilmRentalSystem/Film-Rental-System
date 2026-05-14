using FilmRentalStore.MVC.DTOs.Customers;

namespace FilmRentalStore.MVC.ViewModels.Customer
{
    public class CustomerIndexViewModel
    {
        public List<CustomerResponseDto> Customers { get; set; } = new List<CustomerResponseDto>();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCustomers { get; set; }
        public string? SearchName { get; set; }
        public string? SearchEmail { get; set; }
    }
}
