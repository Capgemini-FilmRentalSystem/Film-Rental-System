using FilmRentalStore.API.DTOs.Address;

namespace FilmRentalStore.API.DTOs
{
    public class RoleSummaryDto
    {
        public string RoleTitle { get; set; } = string.Empty;
    }

    public class StaffSummaryDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string RoleTitle { get; set; } = string.Empty;
    }

    public class StoreSummaryDto
    {
        public StaffSummaryDto ManagerStaff { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }

    public class CustomerSummaryDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class LanguageSummaryDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class FilmSummaryDto
    {
        public string Title { get; set; } = string.Empty;
    }

    public class InventorySummaryDto
    {
        public FilmSummaryDto Film { get; set; } = null!;
    }

    public class RentalSummaryDto
    {
        public DateTime RentalDate { get; set; }
        public FilmSummaryDto Film { get; set; } = null!;
        public DateTime? ReturnDate { get; set; }
    }
}
