using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.DTOs.Store;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    // ── Staff Validators ──────────────────────────────────────────────────────

    public class CreateStaffValidator : AbstractValidator<CreateStaffDto>
    {
        public CreateStaffValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(45).WithMessage("First name cannot exceed 45 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(45).WithMessage("Last name cannot exceed 45 characters.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(16).WithMessage("Username cannot exceed 16 characters.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters.")
                .When(x => x.Email != null);

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(40).WithMessage("Password cannot exceed 40 characters.")
                .When(x => x.Password != null);

            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage("A valid Address ID is required.");

            RuleFor(x => x.StoreId)
                .GreaterThan(0).WithMessage("A valid Store ID is required.");
        }
    }

    public class UpdateStaffValidator : AbstractValidator<UpdateStaffDto>
    {
        public UpdateStaffValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(45).WithMessage("First name cannot exceed 45 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(45).WithMessage("Last name cannot exceed 45 characters.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(16).WithMessage("Username cannot exceed 16 characters.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters.")
                .When(x => x.Email != null);

            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage("A valid Address ID is required.");

            RuleFor(x => x.StoreId)
                .GreaterThan(0).WithMessage("A valid Store ID is required.");
        }
    }

    public class UpdateStaffEmailValidator : AbstractValidator<UpdateStaffEmailDto>
    {
        public UpdateStaffEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters.");
        }
    }

    public class UpdateStaffPasswordValidator : AbstractValidator<UpdateStaffPasswordDto>
    {
        public UpdateStaffPasswordValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters.")
                .MaximumLength(40).WithMessage("New password cannot exceed 40 characters.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        }
    }

    public class UpdateStaffActiveValidator : AbstractValidator<UpdateStaffActiveDto>
    {
        public UpdateStaffActiveValidator()
        {
            RuleFor(x => x.Active)
                .NotNull().WithMessage("Active status is required.");
        }
    }

    // ── Store Validators ──────────────────────────────────────────────────────

    public class CreateStoreValidator : AbstractValidator<CreateStoreDto>
    {
        public CreateStoreValidator()
        {
            RuleFor(x => x.ManagerStaffId)
                .GreaterThan((byte)0).WithMessage("A valid Manager Staff ID is required.");

            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage("A valid Address ID is required.");
        }
    }

    public class UpdateStoreValidator : AbstractValidator<UpdateStoreDto>
    {
        public UpdateStoreValidator()
        {
            RuleFor(x => x.ManagerStaffId)
                .GreaterThan((byte)0).WithMessage("A valid Manager Staff ID is required.");

            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage("A valid Address ID is required.");
        }
    }

    public class UpdateStoreManagerValidator : AbstractValidator<UpdateStoreManagerDto>
    {
        public UpdateStoreManagerValidator()
        {
            RuleFor(x => x.ManagerStaffId)
                .GreaterThan((byte)0).WithMessage("A valid Manager Staff ID is required.");
        }
    }

    public class UpdateStoreAddressValidator : AbstractValidator<UpdateStoreAddressDto>
    {
        public UpdateStoreAddressValidator()
        {
            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage("A valid Address ID is required.");
        }
    }

}
