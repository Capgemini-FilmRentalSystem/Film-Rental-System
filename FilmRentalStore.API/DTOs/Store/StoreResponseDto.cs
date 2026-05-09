namespace FilmRentalStore.API.DTOs.Store
{
    public class StoreResponseDto
    {
        public int StoreId { get; set; }
        public byte ManagerStaffId {  get; set; }
        public string ManagerName { get; set; } = null!;
        public int AddressId { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
