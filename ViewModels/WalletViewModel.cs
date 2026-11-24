namespace EWallet.ViewModels;

public class WalletViewModel
{
  public bool HasAccount { get; set; }

  public string? NumeroCompte { get; set; }

  public decimal Solde { get; set; }

  public string Devise { get; set; } = "MAD";

  public string? Statut { get; set; }

  public DateTime DateCreation { get; set; }

  public string Nom { get; set; } = string.Empty;

  public string Prenom { get; set; } = string.Empty;

  public string Email { get; set; } = string.Empty;

  public string UserName { get; set; } = string.Empty;

  public string? PhoneNumber { get; set; }

  public DateTime UserDateCreation { get; set; }
}