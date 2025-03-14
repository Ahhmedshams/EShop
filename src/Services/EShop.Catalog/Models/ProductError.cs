using ErrorOr;

namespace EShop.Catalog.Models;

public class ProductError
{
  



    public static readonly Error PriceMustBeGreaterThanZero = Error.Validation(
        "Product.PriceMustBeGreaterThanZero",
        "Product price must be greater than zero.");

    public static readonly Error MaxStockThresholdMustBeGreaterThanZero = Error.Validation(
        "Product.MaxStockThresholdMustBeGreaterThanZero",
        "MaxStockThreshold must be greater than zero.");

    public static readonly Error AvailableStockCannotBeNegative = Error.Validation(
        "Product.AvailableStockCannotBeNegative",
        "Available stock cannot be negative.");

    public static readonly Error QuantityMustBeGreaterThanZero = Error.Validation(
        "Product.QuantityMustBeGreaterThanZero",
        "Quantity must be greater than zero.");

    public static readonly Error CannotRemoveStockFromEmptyInventory = Error.Validation(
        "Product.CannotRemoveStockFromEmptyInventory",
        "Cannot remove stock from an empty inventory.");



   


}