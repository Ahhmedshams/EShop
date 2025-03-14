using FluentValidation;

namespace EShop.Catalog.Endpoints.Contracts;



public sealed record UpdateProductMaxStockThresholdRequest(
    string Slug,
    int MaxStockThreshold);

public sealed class UpdateProductMaxStockThresholdRequestValidator : AbstractValidator<UpdateProductMaxStockThresholdRequest>
{
    public UpdateProductMaxStockThresholdRequestValidator()
    {
        RuleFor(x => x.MaxStockThreshold)
            .GreaterThan(0);

        RuleFor(x => x.Slug)
            .MaximumLength(150)
            .NotNull();
    }
}