using FluentValidation;
using MediatR;
using products.Domain.Shared;

namespace products.Domain.Itens.Commands;

public class CreateItemCommand : IRequest<NotificationResult>
{
    public CreateItemCommand(string name, double price)
    {
        Name = name;
        Price = price;
    }

    public string? Name { get; set; }
    public double Price { get; set; }
}
public class CreateItemValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotNull().NotEmpty().Length(2, 100).WithMessage("Informe um nome.");
        RuleFor(x => x.Price).NotNull().NotEmpty().GreaterThan(0).WithMessage("Preço deve ser maior que 0");
        // .ScalePrecision(10, 2).WithMessage("Formato invalido. Use por exemplo 100.90");
    }
}