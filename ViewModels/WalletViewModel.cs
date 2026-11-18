namespace EWallet.ViewModels;

public class WalletViewModel
{
  public bool HasAccount { get; set; }

  public string? NumeroCompte { get; set; }

  public decimal Solde { get; set; }

  public string Devise { get; set; } = "MAD";

  public string? Statut { get; set; }

  public DateTime DateCreation { get; set; }
}