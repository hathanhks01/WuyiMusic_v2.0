using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WuyiMusic_DAL.Migrations
{
    /// <inheritdoc />
    public partial class Updatetrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTracks_Tracks_TrackId",
                table: "PlaylistTracks");

            migrationBuilder.AddColumn<string>(
                name: "MetaLinkImage",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTracks_Tracks_TrackId",
                table: "PlaylistTracks",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTracks_Tracks_TrackId",
                table: "PlaylistTracks");

            migrationBuilder.DropColumn(
                name: "MetaLinkImage",
                table: "Tracks");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTracks_Tracks_TrackId",
                table: "PlaylistTracks",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
