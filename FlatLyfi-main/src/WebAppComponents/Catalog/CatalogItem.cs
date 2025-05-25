namespace eShop.WebAppComponents.Catalog;

public record CatalogItem(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string PictureUrl,
    int CatalogBrandId,
    CatalogBrand CatalogBrand,
    int CatalogTypeId,
    CatalogItemType CatalogType,
    string Link,
    string Region,
    string Town,
    string Metro,
    string TimeToTheMetro,
    string Address,
    string NumberOfRooms,
    string TotalFloorArea,
    string KitchenArea,
    string Floor,
    string LivingArea,
    string Bathroom,
    string Repair,
    string Furniture,
    string Technique,
    string HouseType,
    string InternetAndTV,
    string YearBuilt,
    string FloorsInTheHouse,
    string PassengerElevator,
    string FreightElevator,
    string Parking,
    decimal Longitude,
    decimal Latitude,
    string LinkToProductCard);

public record CatalogResult(int PageIndex, int PageSize, int Count, List<CatalogItem> Data);
public record CatalogBrand(int Id, string Brand);
public record CatalogItemType(int Id, string Type);
