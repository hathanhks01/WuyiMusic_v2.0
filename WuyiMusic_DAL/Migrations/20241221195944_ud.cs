using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WuyiMusic_DAL.Migrations
{
    /// <inheritdoc />
    public partial class ud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("07be34d9-67ff-4de9-a764-4b1275e3fc9c"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("f0f7d448-582c-4f77-bbc9-366cf602e294"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("fd8d03f3-5359-4a09-8270-c2b4fd2f55a3"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { new Guid("58de85c3-30d8-4f2c-940c-002c6bb214e2"), "user" },
                    { new Guid("94a3ea36-b30c-4ad8-8a9e-8262fb030fdc"), "artist" },
                    { new Guid("d1f4eaa0-1b2c-42e8-9ff7-ff6f983ae412"), "admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("58de85c3-30d8-4f2c-940c-002c6bb214e2"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("94a3ea36-b30c-4ad8-8a9e-8262fb030fdc"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("d1f4eaa0-1b2c-42e8-9ff7-ff6f983ae412"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { new Guid("07be34d9-67ff-4de9-a764-4b1275e3fc9c"), "user" },
                    { new Guid("f0f7d448-582c-4f77-bbc9-366cf602e294"), "artist" },
                    { new Guid("fd8d03f3-5359-4a09-8270-c2b4fd2f55a3"), "admin" }
                });
        }
    }
}
