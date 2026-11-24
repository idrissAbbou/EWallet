
using System.Security.Claims;
using EWallet.Data.Repositories;
using EWallet.Models;
using EWallet.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

[Authorize(Roles = "Client")]
public class WalletController : Controller
{
    private readonly AccountRepository _accountRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public WalletController(
        AccountRepository accountRepository,
        UserManager<ApplicationUser> userManager)
    {
        _accountRepository = accountRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userIdValue =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdValue, out int userId))
            return Forbid();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Forbid();

        var account = await _accountRepository.GetByUserIdAsync(userId);

        if (account == null)
        {
            return View(new WalletViewModel
            {
                HasAccount = false,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                UserDateCreation = user.DateCreation
            });
        }

        var model = new WalletViewModel
        {
            HasAccount = true,
            NumeroCompte = account.NumeroCompte,
            Solde = account.Solde,
            Devise = account.Devise,
            Statut = account.Statut,
            DateCreation = account.DateCreation,
            Nom = user.Nom,
            Prenom = user.Prenom,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            UserDateCreation = user.DateCreation
        };

        return View(model);
    }
}
