using FluentValidation;
using OrdersAPI.DTOs;

namespace OrdersAPI.Validators
{
    public class UpdateOrderValidator : AbstractValidator<UpdateOrderDto>
    {
        public UpdateOrderValidator()
        {
            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
                .LessThan(1000000).WithMessage("Valor muito alto");
        }
    }
}