using System.ComponentModel.DataAnnotations;

namespace FilmRentalStore.MVC.ViewModels.Payment
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int RentalId { get; set; }

        [Required]
        [Range(0.01, 999999)]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}