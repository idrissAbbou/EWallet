

namespace EWallet.Models;

public class FraudDetection
{
    public int IdFraude { get; set; }
    public int IdTransaction { get; set; }
    public Transaction? Transaction { get; set; }

    // Score 0 -> 100
    public decimal ScoreRisque { get; set; }

    // Ex: "montant élevé ; nouveau destinataire"
    public string? Raison { get; set; }

    public DateTime DateDetection { get; set; } = DateTime.UtcNow;

    // 3 valeurs possibles: "Normale" / "Suspecte" / "Bloquee"
    // DecisionIA
    public string? DecisionIA { get; set; }
}