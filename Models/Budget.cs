namespace EWallet.Models;

public class Budget
{
  public int IdBudget { get; set; }

  public int IdUser { get; set; }
  public ApplicationUser? User { get; set; }

  public int IdCategorie { get; set; }
  public Category? Category { get; set; }

  public decimal MontantLimite { get; set; }

  public DateTime DateDebut { get; set; }
  public DateTime DateFin { get; set; }
}