using FluentValidation;

namespace EShop.Catalog.Endpoints.Contracts;


public sealed record UpdateProductRequest(
    string slug,
    string Description,
    int CatalogId,
    int BrandId);

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .MaximumLength(5000);

        RuleFor(x => x.CatalogId)
            .NotNull();

        RuleFor(x => x.BrandId)
            .NotNull();

        RuleFor(x => x.slug)
            .MaximumLength(150)
            .NotNull();
    }
}