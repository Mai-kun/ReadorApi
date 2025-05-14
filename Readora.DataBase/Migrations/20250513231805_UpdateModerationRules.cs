using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readora.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModerationRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Books",
                type: "integer",
                maxLength: 25,
                nullable: false,
                defaultValue: 0);
        }
    }
}
