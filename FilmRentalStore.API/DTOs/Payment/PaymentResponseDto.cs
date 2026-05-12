using FilmRentalStore.API.DTOs;

namespace FilmRentalStore.API.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }

        public CustomerSummaryDto Customer { get; set; } = null!;

        public StaffSummaryDto Staff { get; set; } = null!;

        public RentalSummaryDto? Rental { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
