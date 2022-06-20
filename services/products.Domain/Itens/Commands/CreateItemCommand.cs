using FluentValidation;
using MediatR;
using products.Domain.Shared;

namespace products.Domain.Itens.Commands;

public class CreateItemCommand:IRequest<NotificationResult>
{
    public string? Name { get; set; }
    public double Price { get; set; }
}
public class CreateItemValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().Length(2, 100).WithMessage("A name must be provided");
        RuleFor(x => x.Price).NotNull().NotEmpty().GreaterThan(0).WithMessage("Please speciy a price");
    }
}