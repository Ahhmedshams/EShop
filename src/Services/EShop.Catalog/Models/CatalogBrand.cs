namespace EShop.Catalog.Models;

public class CatalogBrand
{
    public int Id { get; set; }
    public string Brand { get; private set; } = null!;

    public void Update(string brand) => Brand = brand;

    public static CatalogBrand Create(string brand)
        => new CatalogBrand
        {
            Brand = brand
        };
}