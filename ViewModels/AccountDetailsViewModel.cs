
namespace EWallet.ViewModels;

public class AccountDetailsViewModel
{
    public int IdAccount { get; set; }
    public int IdUser { get; set; }

    public string NumeroCompte { get; set; } = string.Empty;
    public string TypeAccount { get; set; } = string.Empty;

    public decimal Solde { get; set; }
    public string Devise { get; set; } = "MAD";
    public string Statut { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }

    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
