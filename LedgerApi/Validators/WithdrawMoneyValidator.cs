using FluentValidation;
using LedgerApi.DTOs;

namespace LedgerApi.Validators;

public class WithdrawMoneyValidator : AbstractValidator<WithdrawMoneyDto>
{
    public WithdrawMoneyValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Withdrawal amount must be greater than zero")
            .LessThanOrEqualTo(1000000).WithMessage("Withdrawal amount cannot exceed 10,00,000");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters");
    }
}