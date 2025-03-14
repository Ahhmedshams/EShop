using EShop.Catalog.Models;

namespace EShop.Catalog.Endpoints.Contracts;



public sealed record ProductResponse(
    string Name,
    string Slug,
    string Description,
    int BrandId,
    string BrandName,
    int CategoryId,
    string CategoryName,
    decimal Price,
    int AvailableStock,
    int MaxStockThreshold,
    IReadOnlyCollection<CatalogMedia> medias);