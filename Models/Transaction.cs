namespace EWallet.Models;

public class Transaction
{
    public int IdTransaction { get; set; }

    public string ReferenceTransaction { get; set; } = string.Empty;

    public int IdCompteEmetteur { get; set; }

    public int IdCompteDestinataire { get; set; }

    public int IdCategorie { get; set; }

    public decimal Montant { get; set; }

    public DateTime DateTransaction { get; set; }

    public DateTime? DateValidation { get; set; }

    public string TypeTransaction { get; set; } = "Transfer";

    public string? Description { get; set; }

    public string Statut { get; set; } = "Pending";
}
