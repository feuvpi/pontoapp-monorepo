using FluentValidation;
using PontoAPP.Application.DTOs.Users;

namespace PontoAPP.Application.Validators.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Nome completo é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres");

        RuleFor(x => x.Role)
            .Must(role => new[] { "Employee", "Manager", "Admin" }.Contains(role))
            .WithMessage("Role deve ser: Employee, Manager ou Admin");

        RuleFor(x => x.EmployeeCode)
            .MaximumLength(50).WithMessage("Matrícula deve ter no máximo 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.EmployeeCode));

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Departamento deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Department));
    }
}