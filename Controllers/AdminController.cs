using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EWallet.Data.Repositories;

namespace EWallet.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AccountRepository _accountRepository;
 
    public AdminController(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
 
    public IActionResult Index()
    {
        return Content($"Welcome Admin: {User.Identity?.Name}");
    }


    public async Task<IActionResult> Accounts()

    {

      var accounts = await _accountRepository.GetAllAsync();
 
        return View(accounts);

    }

    public async Task<IActionResult> Details(int id)
    {
      var account = await _accountRepository.GetAccountDetailsAsync(id);

      if (account == null)
    {
        return NotFound();
    }

       return View(account);
    }

[HttpPost]
[ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, string newStatus)
  {
    if (newStatus != "Active" && newStatus != "Blocked")
    {
        return BadRequest();
    }

    bool updated = await _accountRepository.UpdateStatusAsync(id, newStatus);

    if (!updated)
    {
        return NotFound();
    }

    return RedirectToAction(nameof(Details), new { id });
}

[HttpPost]
[ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(int id)
    {
    bool deleted = await _accountRepository.SoftDeleteAsync(id);

    if (!deleted)
    {
        TempData["Error"] =
            "Account cannot be deleted. Check that the balance is zero and the account is not already closed or deleted.";

        return RedirectToAction(nameof(Details), new { id });
    }

    TempData["Success"] = "Account deleted successfully.";

    return RedirectToAction(nameof(Accounts));
}
}
