namespace FilmRentalStore.API.DTOs.Store
{
    // ── Response DTO ───────────────────────────────────────────────────────────
    public class StoreResponseDto
    {
        public int StoreId { get; set; }
        public byte ManagerStaffId { get; set; }
        public int AddressId { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    // ── Response with full details ─────────────────────────────────────────────
    public class StoreDetailResponseDto : StoreResponseDto
    {
        public string ManagerName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int TotalStaff { get; set; }
        public int TotalCustomers { get; set; }
    }

    // ── Create DTO ─────────────────────────────────────────────────────────────
    public class CreateStoreDto
    {
        public byte ManagerStaffId { get; set; }
        public int AddressId { get; set; }
    }

    // ── Update DTO ─────────────────────────────────────────────────────────────
    public class UpdateStoreDto
    {
        public byte ManagerStaffId { get; set; }
        public int AddressId { get; set; }
    }

    // ── Patch DTOs ─────────────────────────────────────────────────────────────
    public class UpdateStoreManagerDto
    {
        public byte ManagerStaffId { get; set; }
    }

    public class UpdateStoreAddressDto
    {
        public int AddressId { get; set; }
    }

    // ── Sales Summary ──────────────────────────────────────────────────────────
    public class StoreSalesDto
    {
        public int StoreId { get; set; }
        public string Store { get; set; } = null!;
        public string Manager { get; set; } = null!;
        public decimal TotalSales { get; set; }
    }

    // ── Staff summary inside store context ────────────────────────────────────
    public class StoreStaffSummaryDto
    {
        public byte StaffId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public bool Active { get; set; }
    }

    // ── Customer summary inside store context ─────────────────────────────────
    public class StoreCustomerSummaryDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string Active { get; set; } = null!;
    }

}
