using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WuyiMusic_DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatealbumimg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrackImage",
                table: "Albums",
                newName: "albumImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "albumImage",
                table: "Albums",
                newName: "TrackImage");
        }
    }
}
