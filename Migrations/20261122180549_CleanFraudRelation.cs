using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWallet.Migrations
{
    /// <inheritdoc />
    public partial class CleanFraudRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_FraudDetections_FraudDetectionIdFraude",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_FraudDetectionIdFraude",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FraudDetectionIdFraude",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FraudDetectionIdFraude",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FraudDetectionIdFraude",
                table: "Transactions",
                column: "FraudDetectionIdFraude");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_FraudDetections_FraudDetectionIdFraude",
                table: "Transactions",
                column: "FraudDetectionIdFraude",
                principalTable: "FraudDetections",
                principalColumn: "IdFraude");
        }
    }
}
