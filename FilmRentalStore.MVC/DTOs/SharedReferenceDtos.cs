using FilmRentalStore.MVC.DTOs.Address;

namespace FilmRentalStore.MVC.DTOs
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


    public class FilmSummaryDto
    {
        public int FilmId { get; set; }
        public string Title { get; set; } = string.Empty;
        public byte RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
    }

    public class InventorySummaryDto
    {
        public int StoreId { get; set; }
        public FilmSummaryDto Film { get; set; } = null!;
    }

    public class RentalSummaryDto
    {
        public int StoreId { get; set; }
        public DateTime RentalDate { get; set; }
        public FilmSummaryDto Film { get; set; } = null!;
        public DateTime? ReturnDate { get; set; }
    }

    public class LanguageSummaryDto
    {
        public byte LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ActorSummaryDto
    {
        public int ActorId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
    }

    public class CategorySummaryDto
    {
        public byte CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
