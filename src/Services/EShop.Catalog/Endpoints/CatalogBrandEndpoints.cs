using EShop.Catalog.Endpoints.Contracts;
using EShop.Catalog.Infrastructure;
using EShop.Catalog.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Endpoints;

public static class CatalogBrandEndpoints
{
    public static IEndpointRouteBuilder MapCatalogBrandEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/brands");
        
        group.MapPost("/", CreateBrand);
        group.MapPut("/", UpdateBrand);
        group.MapDelete("/{id:int:required}", DeleteBrandById);
        group.MapGet("/{id:int:required}", GetBrandById);
        group.MapGet("/", GetBrands);

        return app;
    }
    public static async Task<Results<Created, ValidationProblem, BadRequest<string>>> CreateBrand(
        CatalogDbContext context,
        CreateCatalogBrandRequest brandToCreate,
        IValidator<CreateCatalogBrandRequest> validator,
        CancellationToken cancellationToken)
    {
        var validate = validator.Validate(brandToCreate);
        if (!validate.IsValid)
        {
            return TypedResults.ValidationProblem(validate.ToDictionary());
        }

        var hasBrand = await context.CatalogBrands.AnyAsync(x => x.Brand == brandToCreate.Brand, cancellationToken);
        if (hasBrand)
        {
            return TypedResults.BadRequest($"A brand with the name '{brandToCreate.Brand}' already exists.");
        }

        var brand = CatalogBrand.Create(brandToCreate.Brand);

        context.CatalogBrands.Add(brand);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/catalog/api/v1/brands/{brand.Id}");
    }
    public static async Task<Results<Created, ValidationProblem, NotFound<string>>> UpdateBrand(
        CatalogDbContext context,
        UpdateCatalogBrandRequest brandToUpdate,
        IValidator<UpdateCatalogBrandRequest> validator,
    CancellationToken cancellationToken)
    {
        var validate = validator.Validate(brandToUpdate);
        if (!validate.IsValid)
        {
            return TypedResults.ValidationProblem(validate.ToDictionary());
        }

        var brand = await context.CatalogBrands.FirstOrDefaultAsync(i => i.Id == brandToUpdate.Id, cancellationToken);
        if (brand is null)
        {
            return TypedResults.NotFound($"Brand with id {brandToUpdate.Id} not found.");
        }

        brand.Update(brandToUpdate.Brand);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/catalog/api/v1/brands/{brand.Id}");
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<string>>> DeleteBrandById
        (CatalogDbContext context, int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return TypedResults.BadRequest("Id is not valid.");
        }

        var brand = await context.CatalogBrands.FirstOrDefaultAsync(x => x.Id == id);
        if (brand is null)
        {
            return TypedResults.NotFound();
        }

        context.CatalogBrands.Remove(brand);
        await context.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<CatalogBrandResponse>, NotFound, BadRequest<string>>> GetBrandById(
        CatalogDbContext context, int id)
    {
        if (id <= 0)
        {
            return TypedResults.BadRequest("Id is not valid.");
        }

        var brand = await context.CatalogBrands.FirstOrDefaultAsync(ci => ci.Id == id);
        if (brand is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CatalogBrandResponse(id, brand.Brand));
    }
    public static async Task<Results<Ok<IEnumerable<CatalogBrandResponse>>, BadRequest<string>>> GetBrands(
        CatalogDbContext context,
        CancellationToken cancellationToken)
    {
        var brands = await context.CatalogBrands
            .OrderBy(c => c.Id)
            .Select(x => new CatalogBrandResponse(x.Id, x.Brand))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok<IEnumerable<CatalogBrandResponse>>(brands);
    }
}