namespace EWallet.Models;

public class Transaction
{
  public int IdTransaction { get; set; }

  public int IdAccount { get; set; }
  public Account? Account { get; set; }

  public int IdCategorie { get; set; }
  public Category? Category { get; set; }

  public string TypeTransaction { get; set; } = string.Empty;

  public decimal Montant { get; set; }

  public DateTime DateTransaction { get; set; } = DateTime.UtcNow;

  public string? Description { get; set; }
}