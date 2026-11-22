using EWallet.Data;
using EWallet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Controllers;
public class FraudDetectionController : Controller
{
    private readonly ApplicationDbContext _ctx;
    public FraudDetectionController(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IActionResult> Index()
    {
        var list = await _ctx.FraudDetections
            .Include(f => f.Transaction)
            .OrderByDescending(f => f.DateDetection)
            .ToListAsync();

        ViewBag.TotalAlertes = list.Count;
        ViewBag.Bloquees = list.Count(f => f.DecisionIA == "Bloquee" || f.DecisionIA == "1");
        ViewBag.CetteSemaine = list.Count(f => f.DateDetection >= DateTime.Now.AddDays(-7));
        ViewBag.MontantBloque = await _ctx.Transactions
            .Where(t => t.Statut == "Bloquee")
            .SumAsync(t => (decimal?)t.Montant) ?? 0;
        return View(list);
    }
    private async Task<(int Score, string Raison, string Statut, string Decision)> EvaluerFraude(Transaction tx, Account emetteur)
    {
        int score = 0;
        List<string> raisons = new();

        if (tx.Montant > 10000) { score += 30; raisons.Add("Montant élevé (>10000 MAD)"); }

        var moyenne = await _ctx.Transactions
            .Where(t => t.IdAccountEmetteur == tx.IdAccountEmetteur)
            .AverageAsync(t => (decimal?)t.Montant) ?? 0;
        if (moyenne > 0 && tx.Montant > moyenne * 3) { score += 20; raisons.Add($"Montant anormal (>3x moyenne {moyenne:F0} MAD)"); }

        if (emetteur.Solde > 0 && tx.Montant > emetteur.Solde * 0.9m) { score += 15; raisons.Add("Vidage du compte (>90% solde)"); }

        var nb10min = await _ctx.Transactions
            .CountAsync(t => t.IdAccountEmetteur == tx.IdAccountEmetteur 
                          && t.DateTransaction >= DateTime.Now.AddMinutes(-10));
        if (nb10min > 5) { score += 25; raisons.Add("Vélocité (>5 en 10 min)"); }

        var dejaVu = await _ctx.Transactions.AnyAsync(t => t.IdAccountEmetteur == tx.IdAccountEmetteur 
            && t.IdAccountDestinataire == tx.IdAccountDestinataire 
            && t.IdTransaction != tx.IdTransaction);
        if (!dejaVu) { score += 10; raisons.Add("Nouveau destinataire"); }

        if (tx.DateTransaction.Hour >= 0 && tx.DateTransaction.Hour < 5) { score += 10; raisons.Add("Horaire inhabituel (00h-05h)"); }

        var fractionne = await _ctx.Transactions
            .CountAsync(t => t.IdAccountEmetteur == tx.IdAccountEmetteur 
                          && t.DateTransaction >= DateTime.Now.AddHours(-1)
                          && t.Montant > 8000 && t.Montant < 10000);
        if (fractionne >= 2) { score += 20; raisons.Add("Montants fractionnés"); }

        var dest = await _ctx.Accounts.FindAsync(tx.IdAccountDestinataire);
        if (emetteur.Statut != "Actif" || (dest != null && dest.Statut != "Actif")) 
        { score += 40; raisons.Add("Compte bloqué ou inactif"); }

        var totalJour = await _ctx.Transactions
            .Where(t => t.IdAccountEmetteur == tx.IdAccountEmetteur 
                     && t.DateTransaction >= DateTime.Today 
                     && t.Statut != "Bloquee")
            .SumAsync(t => (decimal?)t.Montant) ?? 0;
        if (totalJour + tx.Montant > 5000) { score += 20; raisons.Add("Limite 5000 MAD/jour dépassée"); }

        if (score > 100) score = 100;
        string raisonFinale = raisons.Count == 0 ? "Aucune anomalie" : string.Join(" ; ", raisons);

        if (score <= 29) return (score, raisonFinale, "Validee", "0");
        if (score <= 69) return (score, raisonFinale, "EnAttente", "1");
        return (score, raisonFinale, "Bloquee", "1");
    }

    public async Task<IActionResult> Test()
    {
        var users = await _ctx.Users.Take(2).ToListAsync();
        if (users.Count < 2) { TempData["Error"] = "Khassek 2 users!"; return RedirectToAction("Index"); }

        var comptes = await _ctx.Accounts.Take(2).ToListAsync();
        if (comptes.Count < 2)
        {
            var c1 = new Account { IdUser = users[0].Id, NumeroCompte = "EWTEST0001", Solde = 50000, Devise = "MAD", Statut = "Actif", TypeAccount = "Courant", DateCreation = DateTime.Now };
            var c2 = new Account { IdUser = users[1].Id, NumeroCompte = "EWTEST0002", Solde = 50000, Devise = "MAD", Statut = "Actif", TypeAccount = "Courant", DateCreation = DateTime.Now };
            _ctx.Accounts.AddRange(c1, c2);
            await _ctx.SaveChangesAsync();
            comptes = await _ctx.Accounts.Take(2).ToListAsync();
        }

        var transaction = new Transaction
        {
            IdAccountEmetteur = comptes[0].IdAccount,
            IdAccountDestinataire = comptes[1].IdAccount,
            IdCategorie = 3,
            Montant = 45000,
            TypeTransaction = "Transfert",
            DateTransaction = DateTime.Today.AddHours(3),
            Statut = "Validee",
            Description = "Test Fraude"
        };
        var resultat = await EvaluerFraude(transaction, comptes[0]);
        transaction.Statut = resultat.Statut;
        _ctx.Transactions.Add(transaction);
        await _ctx.SaveChangesAsync();

        if (resultat.Score >= 30)
        {
            var fake = new FraudDetection
            {
                IdTransaction = transaction.IdTransaction,
                ScoreRisque = resultat.Score,
                Raison = resultat.Raison,
                DecisionIA = resultat.Decision,
                DateDetection = DateTime.Now
            };
            _ctx.FraudDetections.Add(fake);
            await _ctx.SaveChangesAsync();
            TempData["Error"] = $"⚠️ Tentative suspecte détectée! Score {resultat.Score}/100 - {resultat.Raison} - Transaction {resultat.Statut}";
        }
        else
        {
            TempData["Success"] = $"✅ Transaction Validée - Score {resultat.Score}/100 - Aucune anomalie";
        }

        return RedirectToAction("Index");
    }
}