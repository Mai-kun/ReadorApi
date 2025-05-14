using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readora.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                table: "ModerationRequests");

            migrationBuilder.RenameColumn(
                name: "ResolvedAt",
                table: "ModerationRequests",
                newName: "DecisionDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ModerationRequests",
                newName: "RequestDate");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "ModerationRequests",
                newName: "ModeratorComment");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ModerationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ModerationRequests");

            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "ModerationRequests",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModeratorComment",
                table: "ModerationRequests",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "DecisionDate",
                table: "ModerationRequests",
                newName: "ResolvedAt");

            migrationBuilder.AddColumn<string>(
                name: "ModerationStatus",
                table: "ModerationRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
