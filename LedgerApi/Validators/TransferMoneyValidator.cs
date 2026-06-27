using FluentValidation;
using LedgerApi.DTOs;

namespace LedgerApi.Validators;

public class TransferMoneyValidator : AbstractValidator<TransferMoneyDto>
{
    public TransferMoneyValidator()
    {
        RuleFor(x => x.FromAccountId)
            .NotEmpty().WithMessage("Source account ID is required");

        RuleFor(x => x.ToAccountId)
            .NotEmpty().WithMessage("Destination account ID is required");

        RuleFor(x => x)
            .Must(x => x.FromAccountId != x.ToAccountId)
            .WithMessage("Source and destination accounts cannot be the same");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transfer amount must be greater than zero")
            .LessThanOrEqualTo(1000000).WithMessage("Transfer amount cannot exceed 10,00,000");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters");
    }
}