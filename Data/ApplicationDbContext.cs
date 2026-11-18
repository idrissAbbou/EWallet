using EWallet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
  public ApplicationDbContext(
      DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<ApplicationUser>(entity =>
    {
      entity.Property(u => u.Nom)
              .HasMaxLength(80)
              .IsRequired();

      entity.Property(u => u.Prenom)
              .HasMaxLength(80)
              .IsRequired();

      entity.Property(u => u.StatutUser)
              .HasMaxLength(20)
              .HasDefaultValue("Active");

      entity.Property(u => u.DateCreation)
              .HasDefaultValueSql("SYSUTCDATETIME()");
    });
  }
}