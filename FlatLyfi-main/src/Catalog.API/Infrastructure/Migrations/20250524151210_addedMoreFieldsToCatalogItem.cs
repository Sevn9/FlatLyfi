using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Catalog.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedMoreFieldsToCatalogItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bathroom",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FloorsInTheHouse",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreightElevator",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Furniture",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseType",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternetAndTV",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KitchenArea",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LivingArea",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metro",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumberOfRooms",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parking",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassengerElevator",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Repair",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Technique",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeToTheMetro",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalFloorArea",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "Catalog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YearBuilt",
                table: "Catalog",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Bathroom",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "FloorsInTheHouse",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "FreightElevator",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Furniture",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "HouseType",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "InternetAndTV",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "KitchenArea",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "LivingArea",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Metro",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "NumberOfRooms",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Parking",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "PassengerElevator",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Repair",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Technique",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "TimeToTheMetro",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "TotalFloorArea",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "YearBuilt",
                table: "Catalog");
        }
    }
}
