namespace EWallet.Models;

public class FraudDetection
{
    public int IdFraude { get; set; }
    public int IdTransaction { get; set; }
    public Transaction? Transaction { get; set; }
    public decimal ScoreRisque { get; set; }
    public string? Raison { get; set; }
    public DateTime DateDetection { get; set; } = DateTime.UtcNow;
    public string? DecisionIA { get; set; }
}