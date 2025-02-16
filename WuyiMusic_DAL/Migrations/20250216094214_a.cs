using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WuyiMusic_DAL.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suggestions_Tracks_TrackId",
                table: "Suggestions");

            migrationBuilder.AddForeignKey(
                name: "FK_Suggestions_Tracks_TrackId",
                table: "Suggestions",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suggestions_Tracks_TrackId",
                table: "Suggestions");

            migrationBuilder.AddForeignKey(
                name: "FK_Suggestions_Tracks_TrackId",
                table: "Suggestions",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
