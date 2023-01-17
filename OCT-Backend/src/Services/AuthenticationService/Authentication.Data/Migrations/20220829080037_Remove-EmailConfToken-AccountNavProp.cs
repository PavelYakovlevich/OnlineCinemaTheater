using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Data.Migrations
{
    public partial class RemoveEmailConfTokenAccountNavProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmationTokens_Accounts_AccountId1",
                table: "EmailConfirmationTokens");

            migrationBuilder.DropIndex(
                name: "IX_EmailConfirmationTokens_AccountId1",
                table: "EmailConfirmationTokens");

            migrationBuilder.DropColumn(
                name: "AccountId1",
                table: "EmailConfirmationTokens");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId1",
                table: "EmailConfirmationTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationTokens_AccountId1",
                table: "EmailConfirmationTokens",
                column: "AccountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmationTokens_Accounts_AccountId1",
                table: "EmailConfirmationTokens",
                column: "AccountId1",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
