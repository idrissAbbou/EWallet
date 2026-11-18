using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWallet.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
  public IActionResult Index()
  {
    return Content($"Welcome Admin: {User.Identity?.Name}");
  }
}