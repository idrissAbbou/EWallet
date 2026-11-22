using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWallet.Migrations
{
    /// <inheritdoc />
    public partial class FixTransactionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_IdAccount",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "IdAccount",
                table: "Transactions",
                newName: "IdAccountEmetteur");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_IdAccount",
                table: "Transactions",
                newName: "IX_Transactions_IdAccountEmetteur");

            migrationBuilder.AddColumn<int>(
                name: "FraudDetectionIdFraude",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdAccountDestinataire",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FraudDetectionIdFraude",
                table: "Transactions",
                column: "FraudDetectionIdFraude");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdAccountDestinataire",
                table: "Transactions",
                column: "IdAccountDestinataire");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_IdAccountDestinataire",
                table: "Transactions",
                column: "IdAccountDestinataire",
                principalTable: "Accounts",
                principalColumn: "IdAccount",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_IdAccountEmetteur",
                table: "Transactions",
                column: "IdAccountEmetteur",
                principalTable: "Accounts",
                principalColumn: "IdAccount",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_FraudDetections_FraudDetectionIdFraude",
                table: "Transactions",
                column: "FraudDetectionIdFraude",
                principalTable: "FraudDetections",
                principalColumn: "IdFraude");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_IdAccountDestinataire",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_IdAccountEmetteur",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_FraudDetections_FraudDetectionIdFraude",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_FraudDetectionIdFraude",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_IdAccountDestinataire",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FraudDetectionIdFraude",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IdAccountDestinataire",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "IdAccountEmetteur",
                table: "Transactions",
                newName: "IdAccount");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_IdAccountEmetteur",
                table: "Transactions",
                newName: "IX_Transactions_IdAccount");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_IdAccount",
                table: "Transactions",
                column: "IdAccount",
                principalTable: "Accounts",
                principalColumn: "IdAccount",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
