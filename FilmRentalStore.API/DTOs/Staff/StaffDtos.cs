namespace FilmRentalStore.API.DTOs.Staff
{
    // ── Response DTO ───────────────────────────────────────────────────────────
    public class StaffResponseDto
    {
        public byte StaffId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; } = null!;
        public int AddressId { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    // ── Response with full address details ─────────────────────────────────────
    public class StaffDetailResponseDto : StaffResponseDto
    {
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
    }

    // ── Create DTO ─────────────────────────────────────────────────────────────
    public class CreateStaffDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AddressId { get; set; }
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public bool Active { get; set; } = true;
        public string Username { get; set; } = null!;
        public string? Password { get; set; }
    }

    // ── Update DTO ─────────────────────────────────────────────────────────────
    public class UpdateStaffDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AddressId { get; set; }
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; } = null!;
    }

    // ── Patch DTOs ─────────────────────────────────────────────────────────────
    public class UpdateStaffEmailDto
    {
        public string Email { get; set; } = null!;
    }

    public class UpdateStaffPasswordDto
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }

    public class UpdateStaffActiveDto
    {
        public bool Active { get; set; }
    }

}
