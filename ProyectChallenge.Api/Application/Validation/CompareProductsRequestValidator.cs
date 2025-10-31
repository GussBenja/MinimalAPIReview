using FluentValidation;
using ProyectChallenge.Api.Application.Dtos;

namespace ProyectChallenge.Api.Application.Validation
{
    public class CompareProductsRequestValidator : AbstractValidator<CompareProductsRequest>
    {
        public CompareProductsRequestValidator()
        {
            RuleFor(r => r.Ids)
                .NotNull().WithMessage("Debe especificar una lista de IDs.")
                .Must(ids => ids.Count > 0)
                    .WithMessage("Debe especificar al menos un ID de producto.");

            RuleFor(r => r.Ids.Count)
                .GreaterThanOrEqualTo(2)
                .WithMessage("Se requieren al menos 2 productos para comparar.");
        }
    }
}
