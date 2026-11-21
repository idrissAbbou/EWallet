using EWallet.Models;
using Microsoft.AspNetCore.Identity;

namespace EWallet.Services;

public static class IdentitySeeder
{
  public static async Task SeedAsync(IServiceProvider services)
  {
    using var scope = services.CreateScope();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole<int>>>();

    var configuration = scope.ServiceProvider
        .GetRequiredService<IConfiguration>();

    // 1. Ensure roles exist
    string[] roles = { "Admin", "Client" };

    foreach (var role in roles)
    {
      if (!await roleManager.RoleExistsAsync(role))
      {
        var result = await roleManager.CreateAsync(
            new IdentityRole<int>(role));

        EnsureSucceeded(result, $"Creating {role} role");
      }
    }

    // 2. Read Admin credentials from User Secrets
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];
        var nom = configuration["Admin:Nom"] ?? "Administrator";
        var prenom = configuration["Admin:Prenom"] ?? "System";

    if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password))
    {
      throw new InvalidOperationException(
          "Admin credentials are missing from User Secrets.");
    }

    // 3. Find or create Admin
    var admin = await userManager.FindByEmailAsync(email);

    if (admin == null)
    {
      admin = new ApplicationUser
      {
        UserName = email,
        Email = email,
        EmailConfirmed = true,
        Nom = nom,
        Prenom = prenom,
        StatutUser = "Active",
        DateCreation = DateTime.UtcNow
      };

      var result = await userManager.CreateAsync(admin, password);

      EnsureSucceeded(result, "Creating Admin account");
    }

    // 4. Ensure Admin role is assigned
    if (!await userManager.IsInRoleAsync(admin, "Admin"))
    {
      var result = await userManager.AddToRoleAsync(admin, "Admin");

      EnsureSucceeded(result, "Assigning Admin role");
    }
  }

  private static void EnsureSucceeded(
      IdentityResult result,
      string operation)
  {
    if (!result.Succeeded)
    {
      var errors = string.Join(
          "; ",
          result.Errors.Select(e => e.Description));

      throw new InvalidOperationException(
          $"{operation} failed: {errors}");
    }
  }
}