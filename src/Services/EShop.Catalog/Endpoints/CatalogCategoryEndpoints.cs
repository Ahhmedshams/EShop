using EShop.Catalog.Endpoints.Contracts;
using EShop.Catalog.Infrastructure;
using EShop.Catalog.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Endpoints;

public static class CatalogCategoryEndpoints
{
    public static IEndpointRouteBuilder MapCatalogCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        
        var group = app.MapGroup("/api/v1/categories");
        group.MapPost("/", CreateCategory);
        group.MapPut("/", UpdateCategory);
        group.MapDelete("/{id:int:required}", DeleteCategoryById);
        group.MapGet("/{id:int:required}", GetCategoryById);
        group.MapGet("/", GetCategories);

        return app;
    }

    public static async Task<Results<Created, ValidationProblem, BadRequest<string>>> CreateCategory(
        CatalogDbContext context,
        CreateCatalogCategoryRequest categoryToCreate,
        IValidator<CreateCatalogCategoryRequest> validator,
        CancellationToken cancellationToken)
    {
        var validate = validator.Validate(categoryToCreate);
        if (!validate.IsValid)
        {
            return TypedResults.ValidationProblem(validate.ToDictionary());
        }

        if (categoryToCreate.ParentId.HasValue)
        {
            var hasParent = await context.CatalogCategories.AnyAsync(x => x.Id == categoryToCreate.ParentId, cancellationToken);
            if (!hasParent)
            {
                return TypedResults.BadRequest($"A parent Id is not valid.");
            }
        }

        var hasCategory = await context.CatalogCategories.AnyAsync(x => x.Category == categoryToCreate.Category &&
                                                                        x.ParentId == categoryToCreate.ParentId,
                                                                                 cancellationToken);
        if (hasCategory)
        {
            return TypedResults.BadRequest($"A Category with the name '{categoryToCreate.Category}' in this level already exists.");
        }

        var category = CatalogCategory.Create(categoryToCreate.Category, categoryToCreate.ParentId);

        context.CatalogCategories.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/catalog/api/v1/categories/{category.Id}");
    }

    public static async Task<Results<Created, ValidationProblem, NotFound<string>>> UpdateCategory(
        CatalogDbContext context,
    UpdateCatalogCategoryRequest categoryToUpdate,
    IValidator<UpdateCatalogCategoryRequest> validator,
    CancellationToken cancellationToken)
    {
        var validate = validator.Validate(categoryToUpdate);
        if (!validate.IsValid)
        {
            return TypedResults.ValidationProblem(validate.ToDictionary());
        }

        var category = await context.CatalogCategories.FirstOrDefaultAsync(i => i.Id == categoryToUpdate.Id, cancellationToken);
        if (category is null)
        {
            return TypedResults.NotFound($"Category with id {categoryToUpdate.Id} not found.");
        }

        category.Update(categoryToUpdate.Category);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/catalog/api/v1/categories/{category.Id}");
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<string>>> DeleteCategoryById
        ( CatalogDbContext context, int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return TypedResults.BadRequest("Id is not valid.");
        }

        var category = await context.CatalogCategories
                                             .Include(ci => ci.Children)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (category is null)
        {
            return TypedResults.NotFound();
        }

        if (category.Children.Any())
        { 
            return TypedResults.BadRequest("The category has child categories and cannot be deleted.");
        }

        context.CatalogCategories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<CatalogCategoryResponse>, NotFound, BadRequest<string>>> GetCategoryById(
        CatalogDbContext context,
    int id)
    {
        if (id <= 0)
        {
            return TypedResults.BadRequest("Id is not valid.");
        }

        var category = await context.CatalogCategories 
                                             .Include(ci => ci.Parent)             
                                             .FirstOrDefaultAsync(ci => ci.Id == id);
        if (category is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CatalogCategoryResponse(id, category.Category, category.Path));
    }

    public static async Task<Results<Ok<IEnumerable<CatalogCategoryResponse>>, BadRequest<string>>> GetCategories(
        CatalogDbContext context,
    CancellationToken cancellationToken)
    {
        var categories = await context.CatalogCategories
                                       .OrderBy(c => c.Id)
                                       .Select(x => new CatalogCategoryResponse(x.Id, x.Category, x.Path))
                                       .ToListAsync(cancellationToken);

        return TypedResults.Ok<IEnumerable<CatalogCategoryResponse>>(categories);
    }
}
