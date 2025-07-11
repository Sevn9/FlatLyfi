﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Pgvector;

namespace eShop.Catalog.API.Model;

public class CatalogItem
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

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

    public decimal Price { get; set; }

    public string PictureFileName { get; set; }

    public int CatalogTypeId { get; set; }

    public CatalogType CatalogType { get; set; }

    public int CatalogBrandId { get; set; }

    public CatalogBrand CatalogBrand { get; set; }

    // Quantity in stock
    public int AvailableStock { get; set; }

    // Available stock at which we should reorder
    public int RestockThreshold { get; set; }


    // Maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses)
    public int MaxStockThreshold { get; set; }

    /// <summary>Optional embedding for the catalog item's description.</summary>
    [JsonIgnore]
    public Vector Embedding { get; set; }

    /// <summary>
    /// True if item is on reorder
    /// </summary>
    public bool OnReorder { get; set; }

    public CatalogItem() { }


    /// <summary>
    /// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
    /// been breached. If so, a RestockRequest is generated in CheckThreshold. 
    /// 
    /// If there is sufficient stock of an item, then the integer returned at the end of this call should be the same as quantityDesired. 
    /// In the event that there is not sufficient stock available, the method will remove whatever stock is available and return that quantity to the client.
    /// In this case, it is the responsibility of the client to determine if the amount that is returned is the same as quantityDesired.
    /// It is invalid to pass in a negative number. 
    /// </summary>
    /// <param name="quantityDesired"></param>
    /// <returns>int: Returns the number actually removed from stock. </returns>
    /// 
    public int RemoveStock(int quantityDesired)
    {
        if (AvailableStock == 0)
        {
            throw new CatalogDomainException($"Empty stock, product item {Name} is sold out");
        }

        if (quantityDesired <= 0)
        {
            throw new CatalogDomainException($"Item units desired should be greater than zero");
        }

        int removed = Math.Min(quantityDesired, this.AvailableStock);

        this.AvailableStock -= removed;

        return removed;
    }

    /// <summary>
    /// Increments the quantity of a particular item in inventory.
    /// <param name="quantity"></param>
    /// <returns>int: Returns the quantity that has been added to stock</returns>
    /// </summary>
    public int AddStock(int quantity)
    {
        int original = this.AvailableStock;

        // The quantity that the client is trying to add to stock is greater than what can be physically accommodated in the Warehouse
        if ((this.AvailableStock + quantity) > this.MaxStockThreshold)
        {
            // For now, this method only adds new units up maximum stock threshold. In an expanded version of this application, we
            //could include tracking for the remaining units and store information about overstock elsewhere. 
            this.AvailableStock += (this.MaxStockThreshold - this.AvailableStock);
        }
        else
        {
            this.AvailableStock += quantity;
        }

        this.OnReorder = false;

        return this.AvailableStock - original;
    }
}
