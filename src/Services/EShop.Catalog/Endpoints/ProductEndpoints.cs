using EShop.Catalog.Endpoints.Contracts;
using EShop.Catalog.Extensions;
using EShop.Catalog.Infrastructure;
using EShop.Catalog.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/products");

        group.MapPost("/", CreateProduct);
        group.MapPut("/", UpdateProduct);
        group.MapPatch("/max_stock_threshold", UpdateMaxStockThreshold);
        group.MapDelete("/{slug:required}", DeleteProductById);
        group.MapGet("/{slug:required}", GetProductById);
        group.MapGet("/", GetProducts);

        return app;
    }
      public static async Task<Results<Created, ValidationProblem, BadRequest<string>>> CreateProduct(
          CatalogDbContext context,
          CreateProductRequest itemToCreate,
          IValidator<CreateProductRequest> validator,
     // IPublishEndpoint publishEndpoint,
      CancellationToken cancellationToken)
  {
      var validate = validator.Validate(itemToCreate);
      if (!validate.IsValid)
      {
          return TypedResults.ValidationProblem(validate.ToDictionary());
      }

      var hasCategory = await context.CatalogCategories.AnyAsync(x => x.Id == itemToCreate.CatalogId, cancellationToken);
      if (!hasCategory)
      {
          return TypedResults.BadRequest($"A category Id is not valid.");
      }

      var hasBrand = await context.CatalogBrands.AnyAsync(x => x.Id == itemToCreate.BrandId, cancellationToken);
      if (!hasBrand)
      {
          return TypedResults.BadRequest($"A brand Id is not valid.");
      }

      var hasItemSlug = await context.Products.AnyAsync(x => x.Slug == itemToCreate.Name.ToKebabCase(), cancellationToken);
      if (hasItemSlug)
      {
          return TypedResults.BadRequest($"A Item with the slug '{itemToCreate.Name.ToKebabCase()}' already exists.");
      }

      var item = Product.Create(
          itemToCreate.Name,
          itemToCreate.Description,
          itemToCreate.MaxStockThreshold,
          itemToCreate.BrandId, itemToCreate.CatalogId);

      context.Products.Add(item);
      await context.SaveChangesAsync(cancellationToken);

      var detailUrl = $"/catalog/api/v1/products/{item.Slug}";
      var loadedItem = await context.Products
                                                  .Include(ci => ci.Brand)
                                                  .Include(ci => ci.Category)
                                                  .FirstAsync(x => x.Slug == item.Slug);

      // await services.Publish.Publish(new CatalogItemAddedEvent(
      //         loadedItem.Name,
      //         loadedItem.Description,
      //         loadedItem.CatalogCategory.Category,
      //         loadedItem.CatalogBrand.Brand,
      //         loadedItem.Slug,
      //         detailUrl));

      return TypedResults.Created(detailUrl);
  }

  public static async Task<Results<Created, ValidationProblem, NotFound<string>, BadRequest<string>>> UpdateProduct(
      CatalogDbContext context,
      UpdateProductRequest itemToUpdate,
  IValidator<UpdateProductRequest> validator,
  CancellationToken cancellationToken)
  {
      var validate = validator.Validate(itemToUpdate);
      if (!validate.IsValid)
      {
          return TypedResults.ValidationProblem(validate.ToDictionary());
      }

      var Item = await context.Products
                                              .FirstOrDefaultAsync(i => i.Slug == itemToUpdate.slug, cancellationToken);
      if (Item is null)
      {
          return TypedResults.NotFound($"Item with slug {itemToUpdate.slug} not found.");
      }

      var hasCategory = await context.CatalogCategories.AnyAsync(x => x.Id == itemToUpdate.CatalogId, cancellationToken);
      if (!hasCategory)
      {
          return TypedResults.BadRequest($"A category Id is not valid.");
      }

      var hasBrand = await context.CatalogBrands.AnyAsync(x => x.Id == itemToUpdate.BrandId, cancellationToken);
      if (!hasBrand)
      {
          return TypedResults.BadRequest($"A brand Id is not valid.");
      }

      Item.Update(itemToUpdate.Description,
                  itemToUpdate.BrandId,
                  itemToUpdate.CatalogId);

      await context.SaveChangesAsync(cancellationToken);

      var loadedItem = await context.Products
                                          .Include(ci => ci.Brand)
                                          .Include(ci => ci.Category)
                                          .FirstAsync(x => x.Slug == Item.Slug);

      var detailUrl = $"/catalog/api/v1/products/{loadedItem.Slug}";

      // await services.Publish.Publish(new CatalogItemChangedEvent(
      //         loadedItem.Name,
      //         loadedItem.Description,
      //         loadedItem.CatalogCategory.Category,
      //         loadedItem.CatalogBrand.Brand,
      //         loadedItem.Slug,
      //         detailUrl));

      return TypedResults.Created(detailUrl);
  }

  public static async Task<Results<Created, ValidationProblem, NotFound<string>, BadRequest<string>>> UpdateMaxStockThreshold(
      CatalogDbContext context,
      UpdateProductMaxStockThresholdRequest itemToUpdate,
  IValidator<UpdateProductMaxStockThresholdRequest> validator,
  CancellationToken cancellationToken)
  {
      var validate = validator.Validate(itemToUpdate);
      if (!validate.IsValid)
      {
          return TypedResults.ValidationProblem(validate.ToDictionary());
      }

      var Item = await context.Products.FirstOrDefaultAsync(i => i.Slug == itemToUpdate.Slug, cancellationToken);
      if (Item is null)
      {
          return TypedResults.NotFound($"Item with Slug {itemToUpdate.Slug} not found.");
      }

      Item.SetMaxStockThreshold(itemToUpdate.MaxStockThreshold);

      await context.SaveChangesAsync(cancellationToken);

      return TypedResults.Created($"/catalog/api/v1/products/{Item.Slug}");
  }

  public static async Task<Results<NoContent, NotFound, BadRequest<string>>> DeleteProductById(
      CatalogDbContext context,
  string slug,
  CancellationToken cancellationToken)
  {
      if (string.IsNullOrEmpty(slug))
      {
          return TypedResults.BadRequest("Slug is not valid.");
      }

      var item = await context.Products.FirstOrDefaultAsync(x => x.Slug == slug);
      if (item is null)
      {
          return TypedResults.NotFound();
      }

      context.Products.Remove(item);
      await context.SaveChangesAsync(cancellationToken);
      return TypedResults.NoContent();
  }

  public static async Task<Results<Ok<ProductResponse>, NotFound, BadRequest<string>>> GetProductById(
      CatalogDbContext context,
  string slug)
  {
      if (string.IsNullOrEmpty(slug))
      {
          return TypedResults.BadRequest("Slug is not valid.");
      }

      var item = await context.Products
                                       .Include(x => x.Brand)
                                       .Include(x => x.Category)
                                       .Include(x => x.Medias)
                                       .FirstOrDefaultAsync(ci => ci.Slug == slug);
      if (item is null)
      {
          return TypedResults.NotFound();
      }

      return TypedResults.Ok(
          new ProductResponse(
              item.Name,
              item.Slug,
              item.Description,
              item.BrandId,
              item.Brand.Brand,
              item.CategoryId,
              item.Category.Category,
              item.Price,
              item.AvailableStock,
              item.MaxStockThreshold, [.. item.Medias]));
  }

  public static async Task<Results<Ok<IEnumerable<ProductResponse>>, BadRequest<string>>> GetProducts(
      CatalogDbContext context,
  CancellationToken cancellationToken)
  {
      var items = (await context.Products
                                        .AsNoTracking()
                                        .Include(x => x.Brand)
                                        .Include(x => x.Category)
                                        .OrderBy(c => c.Name)
                                        .ToListAsync(cancellationToken))
                                        .Select(x => new ProductResponse(x.Name,
                                                                             x.Slug,
                                                                             x.Description,
                                                                             x.BrandId,
                                                                             x.Brand.Brand,
                                                                             x.CategoryId,
                                                                             x.Category.Category,
                                                                             x.Price,
                                                                             x.AvailableStock,
                                                                             x.MaxStockThreshold, [.. x.Medias]))
                                        ;

      return TypedResults.Ok<IEnumerable<ProductResponse>>(items);
  }
}