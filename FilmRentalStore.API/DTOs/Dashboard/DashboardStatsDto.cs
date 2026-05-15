namespace FilmRentalStore.API.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int FilmCount { get; set; }
        public int StoreCount { get; set; }
        public int AvailableCopyCount { get; set; }
        public int ActiveRentalCount { get; set; }
        public int ActiveStaffCount { get; set; }
        public int ActiveCustomerCount { get; set; }
        public int PaymentCount { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}
