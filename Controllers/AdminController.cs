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
}
