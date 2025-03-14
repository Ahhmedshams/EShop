using ErrorOr;
using EShop.Catalog.Extensions;

namespace EShop.Catalog.Models;

public class Product
{
    public string Name { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public decimal Price { get; private set; }

    public int AvailableStock { get; private set; }
    
    public string Slug { get; private set; } = null!;

    public int MaxStockThreshold { get; private set; }
    public int BrandId { get; private set; }
    public int CategoryId { get; private set; }
    
    
    public ICollection<CatalogMedia> Medias { get; private set; } = [];

    public CatalogBrand Brand { get; private set; } = null!;
    public CatalogCategory Category { get; private set; } = null!;

    public static Product Create(string name, string description, int maxStockThreshold, int brandId, int categoryId, decimal price = default)
    {
        var newItem = new Product
        {
            Name = name,
            BrandId = brandId,
            CategoryId = categoryId,
            Description = description,
            Slug = name.ToKebabCase(),
            Price = price
        };

        newItem.SetMaxStockThreshold(maxStockThreshold);

        return newItem;
    }

    public void Update(string description, int brandId, int categoryId)
    {
        BrandId = brandId;
        CategoryId = categoryId;
        Description = description;
    }
    public ErrorOr<Success> SetMaxStockThreshold(int maxStockThreshold)
    {
        if (maxStockThreshold <= 0)
            return ProductError.MaxStockThresholdMustBeGreaterThanZero;

        MaxStockThreshold = maxStockThreshold;
        return Result.Success;
    }
    public ErrorOr<int> RemoveStock(int quantity)
    {
        if (AvailableStock == 0)
        {
            return ProductError.CannotRemoveStockFromEmptyInventory;
        }

        if (quantity <= 0)
        {
            return ProductError.QuantityMustBeGreaterThanZero;

        }

        int removed = Math.Min(quantity, AvailableStock);

        AvailableStock -= removed;
        return removed;
    }

    public int AddStock(int quantity)
    {
        int original = AvailableStock;

        if ((AvailableStock + quantity) > MaxStockThreshold)
        {
            AvailableStock += (MaxStockThreshold - AvailableStock);
        }
        else
        {
            AvailableStock += quantity;
        }

        return AvailableStock - original;
    }

    public ErrorOr<Success> UpdatePrice(decimal price)
    {
        if (price <= 0)
           return  ProductError.PriceMustBeGreaterThanZero;

        Price = price;
        return Result.Success;
    }

    public void AddMedia(string fileName, string url)
    {
        Medias.Add(new CatalogMedia(fileName, url));
    }
    
}




// //Summary of Business Invariants:
// Every catalog item must have a name and description, which cannot be changed after creation.
//     The price of an item must always be greater than zero.
//     The maximum stock threshold must be a positive number.
//     Available stock cannot exceed the maximum stock threshold.
//     Stock levels cannot go negative.
//     Stock adjustments must involve positive quantities.
//     Stock cannot be removed if the item is out of stock.
//     Every item must belong to a brand and category.
//     Media files can only be added, not removed or replaced.
//     The slug (URL-friendly name) cannot be changed after creation.
//     Brand and category associations cannot be changed after creation.
