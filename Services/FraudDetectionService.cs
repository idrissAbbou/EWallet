using EWallet.Data;
using EWallet.Models;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Services;

public class FraudDetectionService(ApplicationDbContext ctx)
{
    public async Task<FraudDetection> AnalyserAsync(Transaction t)
    {
        decimal score = 0;
        var raisons = new List<string>();
        void Add(decimal pts, string raison, bool cond)
        {
            if (cond) { score += pts; raisons.Add(raison); }
        }

        var emetteur = await ctx.Accounts.FindAsync(t.IdAccountEmetteur);
        var dest = await ctx.Accounts.FindAsync(t.IdAccountDestinataire);
        var hist = await ctx.Transactions.Where(x => x.IdAccountEmetteur == t.IdAccountEmetteur).ToListAsync();
        var moyenne = hist.Any() ? hist.Average(x => x.Montant) : 0;

        Add(30, "montant élevé", t.Montant > 10000);
        Add(20, "montant anormal (3x moyenne)", moyenne > 0 && t.Montant > moyenne * 3);
        Add(15, "vidage compte (>90%)", emetteur != null && t.Montant > emetteur.Solde * 0.9m);
        Add(25, "fréquence élevée", hist.Count(x => x.DateTransaction >= DateTime.UtcNow.AddMinutes(-10)) + 1 > 5);
        Add(10, "nouveau destinataire", !hist.Any(x => x.IdAccountDestinataire == t.IdAccountDestinataire));
        Add(10, "horaire inhabituel", DateTime.UtcNow.Hour < 5);
        Add(20, "montants fractionnés", hist.Count(x => x.Montant >= 9000 && x.Montant < 10000 && x.DateTransaction >= DateTime.UtcNow.AddHours(-1)) >= 2);
     
var userEmetteur = emetteur != null ? await ctx.Users.FindAsync(emetteur.IdUser) : null;
var userDest = dest != null ? await ctx.Users.FindAsync(dest.IdUser) : null;
Add(40, "compte bloqué", userEmetteur?.StatutUser != "Active" || userDest?.StatutUser != "Active");

        if (score > 100) score = 100;

        var decision = score <= 29 ? "Normale" : score <= 69 ? "Suspecte" : "Bloquee";
        t.Statut = score <= 29 ? "Validée" : score <= 69 ? "EnAttente" : "Bloquée";

        return new FraudDetection
        {
            IdTransaction = t.IdTransaction,
            ScoreRisque = score,
            Raison = raisons.Count == 0 ? "aucune règle" : string.Join(" ; ", raisons),
            DateDetection = DateTime.UtcNow,
            DecisionIA = decision
        };
    }
}