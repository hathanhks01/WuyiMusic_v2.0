using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WuyiMusic_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQueueCurrentTrackOnDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queues_Tracks_CurrentTrackId",
                table: "Queues");

            migrationBuilder.AddForeignKey(
                name: "FK_Queues_Tracks_CurrentTrackId",
                table: "Queues",
                column: "CurrentTrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queues_Tracks_CurrentTrackId",
                table: "Queues");

            migrationBuilder.AddForeignKey(
                name: "FK_Queues_Tracks_CurrentTrackId",
                table: "Queues",
                column: "CurrentTrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId");
        }
    }
}
