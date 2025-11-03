using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoDrinkShop.Migrations.AppIdentityDb
{
    /// <inheritdoc />
    public partial class AddedMorePropertiesToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VerifyByEmail",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerifyByEmail",
                table: "AspNetUsers");
        }
    }
}
