﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readora.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class IntegrateBlockchain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModerationRequests_Users_ModeratorId",
                table: "ModerationRequests");

            migrationBuilder.DropColumn(
                name: "BlockNumber",
                table: "BlockchainTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModeratorId",
                table: "ModerationRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModerationRequests_Users_ModeratorId",
                table: "ModerationRequests",
                column: "ModeratorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModerationRequests_Users_ModeratorId",
                table: "ModerationRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModeratorId",
                table: "ModerationRequests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "BlockNumber",
                table: "BlockchainTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_ModerationRequests_Users_ModeratorId",
                table: "ModerationRequests",
                column: "ModeratorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
