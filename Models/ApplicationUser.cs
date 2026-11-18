using Microsoft.AspNetCore.Identity;

namespace EWallet.Models;

public class ApplicationUser : IdentityUser<int>
{
  public string Nom { get; set; } = string.Empty;

  public string Prenom { get; set; } = string.Empty;

  public string StatutUser { get; set; } = "Active";

  public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}