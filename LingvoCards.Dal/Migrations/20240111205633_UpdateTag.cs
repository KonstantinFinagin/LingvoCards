using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingvoCards.Dal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "TagText",
                table: "Tags",
                newName: "Text");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Tags",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Tags",
                newName: "TagText");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tags",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
