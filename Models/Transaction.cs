namespace EWallet.Models;

public class Transaction
{
    public int IdTransaction { get; set; }

    // ---   (Émetteur) ---
    public int IdAccountEmetteur { get; set; }
    public Account? AccountEmetteur { get; set; }

    // ---  Destinataire  ---
    public int IdAccountDestinataire { get; set; }
    public Account? AccountDestinataire { get; set; }

    // --- Catégorie ---
    public int IdCategorie { get; set; }
    public Category? Category { get; set; }

    public string TypeTransaction { get; set; } = string.Empty;

    public decimal Montant { get; set; }

    public DateTime DateTransaction { get; set; } = DateTime.UtcNow;

    public string? Description { get; set; }

    // Validée / EnAttente / Bloquée
    public string Statut { get; set; } = "EnAttente";

    
}