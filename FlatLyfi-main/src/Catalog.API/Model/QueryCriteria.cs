namespace eShop.Catalog.API.Model;

// Эта запись (record) будет автоматически заполняться из параметров запроса,
// например: /items/filtered?typeId=1&brandId=2
public record QueryCriteria(
        string NumberOfRooms,
        string Floor
        );
