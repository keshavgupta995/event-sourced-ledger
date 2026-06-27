using FluentValidation;
using LedgerApi.DTOs;

namespace LedgerApi.Validators;

public class OpenAccountValidator : AbstractValidator<OpenAccountDto>
{
    public OpenAccountValidator()
    {
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required")
            .MinimumLength(2).WithMessage("Owner name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Owner name cannot exceed 100 characters");

        RuleFor(x => x.InitialDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Initial deposit cannot be negative");
    }
}