using FluentValidation;
using PontoAPP.Application.DTOs.Tenants;

namespace PontoAPP.Application.Validators.Tenants;

/// <summary>
/// Validator for tenant creation request
/// </summary>
public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required")
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .MinimumLength(2).WithMessage("Slug must be at least 2 characters")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters")
            .Matches("^[a-z0-9-_]+$").WithMessage("Slug can only contain lowercase letters, numbers, hyphens and underscores")
            .Must(NotStartOrEndWithSpecialChar).WithMessage("Slug cannot start or end with hyphen or underscore");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.CompanyDocument)
            .MaximumLength(20).WithMessage("Company document must not exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CompanyDocument));
    }

    private bool NotStartOrEndWithSpecialChar(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        return !slug.StartsWith('-') && !slug.StartsWith('_') &&
               !slug.EndsWith('-') && !slug.EndsWith('_');
    }
}