using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWallet.Migrations
{
    /// <inheritdoc />
    public partial class AddFraudDetection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FraudDetections",
                columns: table => new
                {
                    IdFraude = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTransaction = table.Column<int>(type: "int", nullable: false),
                    ScoreRisque = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Raison = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDetection = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecisionIA = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudDetections", x => x.IdFraude);
                    table.ForeignKey(
                        name: "FK_FraudDetections_Transactions_IdTransaction",
                        column: x => x.IdTransaction,
                        principalTable: "Transactions",
                        principalColumn: "IdTransaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FraudDetections_IdTransaction",
                table: "FraudDetections",
                column: "IdTransaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FraudDetections");
        }
    }
}
