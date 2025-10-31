using FluentValidation;
using ProyectChallenge.Api.Application.Dtos;

namespace ProyectChallenge.Api.Application.Validation;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(p => p.MaxPrice)
             .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

        RuleFor(p => p.MinPrice)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

        RuleFor(p => p.MaxPrice)
                .GreaterThanOrEqualTo(p => p.MinPrice)
                .WithMessage("El precio máximo no puede ser menor que el precio mínimo.");

    }
}
