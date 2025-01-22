using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairDataGetter.Server.Migrations
{
    /// <inheritdoc />
    public partial class ConvertGUIDToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUuid",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Customers",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Customers");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageUuid",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
