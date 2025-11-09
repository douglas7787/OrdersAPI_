using FluentValidation;
using OrdersAPI.DTOs;

namespace OrdersAPI.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Nome do cliente é obrigatório")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
                .LessThan(1000000).WithMessage("Valor muito alto");
        }
    }
}