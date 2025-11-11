using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoDrinkShop.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordsHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordsHistory",
                columns: table => new
                {
                    IterationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordsHistory", x => new { x.UserId, x.IterationId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordsHistory");
        }
    }
}
