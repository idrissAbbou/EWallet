namespace EWallet.Models;

public class Account
{
  public int IdAccount { get; set; }

  public int IdUser { get; set; }

  public string NumeroCompte { get; set; } = string.Empty;

  public string TypeAccount { get; set; } = "Wallet";

  public decimal Solde { get; set; }

  public string Devise { get; set; } = "MAD";

  public DateTime DateCreation { get; set; }

  public string Statut { get; set; } = "Active";
}