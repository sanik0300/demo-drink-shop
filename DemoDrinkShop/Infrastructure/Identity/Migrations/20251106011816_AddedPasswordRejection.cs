using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoDrinkShop.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordRejection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UselessPasswords",
                columns: table => new
                {
                    HashedValue = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UselessPasswords", x => x.HashedValue);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UselessPasswords");
        }
    }
}
