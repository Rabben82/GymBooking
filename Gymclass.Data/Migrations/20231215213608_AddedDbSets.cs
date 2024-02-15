using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymClass.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserGymClass");

            migrationBuilder.CreateTable(
                name: "ApplicationUserGymClasses",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GymClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserGymClasses", x => new { x.ApplicationUserId, x.GymClassId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserGymClasses_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserGymClasses_GymClasses_GymClassId",
                        column: x => x.GymClassId,
                        principalTable: "GymClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserGymClasses_GymClassId",
                table: "ApplicationUserGymClasses",
                column: "GymClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserGymClasses");

            migrationBuilder.CreateTable(
                name: "ApplicationUserGymClass",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GymClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserGymClass", x => new { x.ApplicationUserId, x.GymClassId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserGymClass_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserGymClass_GymClasses_GymClassId",
                        column: x => x.GymClassId,
                        principalTable: "GymClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserGymClass_GymClassId",
                table: "ApplicationUserGymClass",
                column: "GymClassId");
        }
    }
}
