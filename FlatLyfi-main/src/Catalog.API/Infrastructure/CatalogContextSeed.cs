using System.Text.Json;
using eShop.Catalog.API.Services;
using Pgvector;

namespace eShop.Catalog.API.Infrastructure;

public partial class CatalogContextSeed(
    IWebHostEnvironment env,
    IOptions<CatalogOptions> settings,
    ICatalogAI catalogAI,
    ILogger<CatalogContextSeed> logger) : IDbSeeder<CatalogContext>
{
    public async Task SeedAsync(CatalogContext context)
    {
        var useCustomizationData = settings.Value.UseCustomizationData;
        var contentRootPath = env.ContentRootPath;
        var picturePath = env.WebRootPath;

        // Workaround from https://github.com/npgsql/efcore.pg/issues/292#issuecomment-388608426
        context.Database.OpenConnection();
        ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypes();

        if (!context.CatalogItems.Any())
        {
            var sourcePath = Path.Combine(contentRootPath, "Setup", "catalog.json");
            var sourceJson = File.ReadAllText(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<CatalogSourceEntry[]>(sourceJson);

            context.CatalogBrands.RemoveRange(context.CatalogBrands);
            await context.CatalogBrands.AddRangeAsync(sourceItems.Select(x => x.Brand).Distinct()
                .Select(brandName => new CatalogBrand { Brand = brandName }));
            logger.LogInformation("Seeded catalog with {NumBrands} brands", context.CatalogBrands.Count());

            context.CatalogTypes.RemoveRange(context.CatalogTypes);
            await context.CatalogTypes.AddRangeAsync(sourceItems.Select(x => x.Type).Distinct()
                .Select(typeName => new CatalogType { Type = typeName }));
            logger.LogInformation("Seeded catalog with {NumTypes} types", context.CatalogTypes.Count());

            await context.SaveChangesAsync();

            var brandIdsByName = await context.CatalogBrands.ToDictionaryAsync(x => x.Brand, x => x.Id);
            var typeIdsByName = await context.CatalogTypes.ToDictionaryAsync(x => x.Type, x => x.Id);

            var catalogItems = sourceItems.Select(source => new CatalogItem
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                Price = source.Price,
                CatalogBrandId = brandIdsByName[source.Brand],
                CatalogTypeId = typeIdsByName[source.Type],
                AvailableStock = 100,
                MaxStockThreshold = 200,
                RestockThreshold = 10,
                PictureFileName = $"{source.Id}.webp",

                Link = source.Link,
                Region = source.Region,
                Town = source.Town,
                Metro = source.Metro,
                TimeToTheMetro = source.TimeToTheMetro,
                Address = source.Address,
                NumberOfRooms = source.NumberOfRooms,
                TotalFloorArea = source.TotalFloorArea,
                KitchenArea = source.KitchenArea,
                Floor = source.Floor,
                LivingArea = source.LivingArea,
                Bathroom = source.Bathroom,
                Repair = source.Repair,
                Furniture = source.Furniture,
                Technique = source.Technique,
                HouseType = source.HouseType,
                InternetAndTV = source.InternetAndTV,
                YearBuilt = source.YearBuilt,
                FloorsInTheHouse = source.FloorsInTheHouse,
                PassengerElevator = source.PassengerElevator,
                FreightElevator = source.FreightElevator,
                Parking = source.Parking,
                Longitude = source.Longitude,
                Latitude = source.Latitude,
                LinkToProductCard = source.LinkToProductCard
            }).ToArray();

            if (catalogAI.IsEnabled)
            {
                logger.LogInformation("Generating {NumItems} embeddings", catalogItems.Length);
                IReadOnlyList<Vector> embeddings = await catalogAI.GetEmbeddingsAsync(catalogItems);
                for (int i = 0; i < catalogItems.Length; i++)
                {
                    catalogItems[i].Embedding = embeddings[i];
                }
            }
            
            await context.CatalogItems.AddRangeAsync(catalogItems);
            logger.LogInformation("Seeded catalog with {NumItems} items", context.CatalogItems.Count());
            await context.SaveChangesAsync();
        }
    }

    private class CatalogSourceEntry
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public string Link { get; set; }

        public string Region { get; set; }

        public string Town { get; set; }

        public string Metro { get; set; }

        public string TimeToTheMetro { get; set; }

        public string Address { get; set; }

        public string NumberOfRooms { get; set; }

        public string TotalFloorArea { get; set; }

        public string KitchenArea { get; set; }

        public string Floor { get; set; }

        public string LivingArea { get; set; }

        public string Bathroom { get; set; }

        public string Repair { get; set; }

        public string Furniture { get; set; }

        public string Technique { get; set; }

        public string HouseType { get; set; }

        public string InternetAndTV { get; set; }

        public string YearBuilt { get; set; }

        public string FloorsInTheHouse { get; set; }

        public string PassengerElevator { get; set; }

        public string FreightElevator { get; set; }

        public string Parking { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }

        public string LinkToProductCard { get; set; }
    }
}
