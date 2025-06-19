namespace eShop.WebApp.Services;

public class BasketItem
{
    public required string Id { get; set; }
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public int Quantity { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public required string PictureUrl { get; set; }
    public int CatalogBrandId { get; set; }
    public int CatalogTypeId { get; set; }
    public required string Link { get; set; }
    public required string Region { get; set; }
    public required string Town { get; set; }
    public required string Metro { get; set; }
    public required string TimeToTheMetro { get; set; }
    public required string Address { get; set; }
    public required string NumberOfRooms { get; set; }
    public required string TotalFloorArea { get; set; }
    public required string KitchenArea { get; set; }
    public required string Floor { get; set; }
    public required string LivingArea { get; set; }
    public required string Bathroom { get; set; }
    public required string Repair { get; set; }
    public required string Furniture { get; set; }
    public required string Technique { get; set; }
    public required string HouseType { get; set; }
    public required string InternetAndTV { get; set; }
    public required string YearBuilt { get; set; }
    public required string FloorsInTheHouse { get; set; }
    public required string PassengerElevator { get; set; }
    public required string FreightElevator { get; set; }
    public required string Parking { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public required string LinkToProductCard { get; set; }
}
