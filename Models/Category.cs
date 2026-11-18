namespace EWallet.Models;

public class Category
{
  public int IdCategorie { get; set; }

  public string NomCategorie { get; set; } = string.Empty;

  public string TypeCategorie { get; set; } = string.Empty;

  public string? Description { get; set; }
}