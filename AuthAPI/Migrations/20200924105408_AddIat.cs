using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthAPI.Migrations
{
    public partial class AddIat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ExpiryDate",
                table: "Tokens",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<long>(
                name: "IssuedAt",
                table: "Tokens",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuedAt",
                table: "Tokens");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "Tokens",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
