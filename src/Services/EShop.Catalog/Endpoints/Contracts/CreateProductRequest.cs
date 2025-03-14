using FluentValidation;

namespace EShop.Catalog.Endpoints.Contracts;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    int CatalogId,
    int BrandId,
    int MaxStockThreshold);

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .MaximumLength(5000);

        RuleFor(x => x.CatalogId)
            .NotNull();

        RuleFor(x => x.BrandId)
            .NotNull();

        RuleFor(x => x.MaxStockThreshold)
            .GreaterThan(0);
    }
}