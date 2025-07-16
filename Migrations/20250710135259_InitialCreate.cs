using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sahalar",
                columns: table => new
                {
                    SahaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sahaAdı = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sahalar", x => x.SahaId);
                });

            migrationBuilder.CreateTable(
                name: "Kuyular",
                columns: table => new
                {
                    KuyuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kuyuAdı = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SahaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kuyular", x => x.KuyuId);
                    table.ForeignKey(
                        name: "FK_Kuyular_Sahalar_SahaId",
                        column: x => x.SahaId,
                        principalTable: "Sahalar",
                        principalColumn: "SahaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wellbores",
                columns: table => new
                {
                    WellboreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    wellboreName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KuyuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wellbores", x => x.WellboreId);
                    table.ForeignKey(
                        name: "FK_Wellbores_Kuyular_KuyuId",
                        column: x => x.KuyuId,
                        principalTable: "Kuyular",
                        principalColumn: "KuyuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kuyular_SahaId",
                table: "Kuyular",
                column: "SahaId");

            migrationBuilder.CreateIndex(
                name: "IX_Wellbores_KuyuId",
                table: "Wellbores",
                column: "KuyuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wellbores");

            migrationBuilder.DropTable(
                name: "Kuyular");

            migrationBuilder.DropTable(
                name: "Sahalar");
        }
    }
}
