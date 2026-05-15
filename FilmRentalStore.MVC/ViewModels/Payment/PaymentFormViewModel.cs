using FilmRentalStore.MVC.DTOs.Payment;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Payment
{
    public class PaymentFormViewModel
    {
        public PaymentRequestDto Payment { get; set; } = new();
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Staff { get; set; } = new();
        public List<SelectListItem> Rentals { get; set; } = new();
    }
}
