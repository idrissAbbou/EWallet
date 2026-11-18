using System.Security.Claims;
using EWallet.Data.Repositories;
using EWallet.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

[Authorize(Roles = "Client")]
public class WalletController : Controller
{
  private readonly AccountRepository _accountRepository;

  public WalletController(AccountRepository accountRepository)
  {
    _accountRepository = accountRepository;
  }

  public async Task<IActionResult> Index()
  {
    // Get the authenticated user ID
    var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdValue, out int userId))
      return Forbid();

    //ADO.NET
    var account = await _accountRepository.GetByUserIdAsync(userId);

    // if client doesn't have a wallet then will return an empty Wallet view Model
    if (account == null)
    {
      return View(new WalletViewModel
      {
        HasAccount = false
      });
    }

    // else we store data in the viewModel for the dashboard
    var model = new WalletViewModel
    {
      HasAccount = true,
      NumeroCompte = account.NumeroCompte,
      Solde = account.Solde,
      Devise = account.Devise,
      Statut = account.Statut,
      DateCreation = account.DateCreation
    };

    return View(model);
  }
}