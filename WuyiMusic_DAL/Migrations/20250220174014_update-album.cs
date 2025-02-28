using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WuyiMusic_DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatealbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaLinkImage",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackImage",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaLinkImage",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "TrackImage",
                table: "Albums");
        }
    }
}
