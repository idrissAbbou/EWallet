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

  public DbSet<Account> Accounts => Set<Account>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Transaction> Transactions => Set<Transaction>();
  public DbSet<Budget> Budgets => Set<Budget>();
  
  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    builder.Entity<Account>().HasKey(a => a.IdAccount);
    builder.Entity<Account>().Property(a => a.Solde).HasPrecision(18, 2);

    builder.Entity<Category>().HasKey(c => c.IdCategorie);
    builder.Entity<Category>().Property(c => c.NomCategorie).HasMaxLength(80).IsRequired();
    builder.Entity<Category>().Property(c => c.TypeCategorie).HasMaxLength(20).IsRequired();

    builder.Entity<Transaction>().HasKey(t => t.IdTransaction);
    builder.Entity<Transaction>().Property(t => t.Montant).HasPrecision(18, 2);
    builder.Entity<Transaction>().Property(t => t.TypeTransaction).HasMaxLength(20).IsRequired();
    builder.Entity<Transaction>().HasOne(t => t.Account).WithMany().HasForeignKey(t => t.IdAccount).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<Transaction>().HasOne(t => t.Category).WithMany().HasForeignKey(t => t.IdCategorie).OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Budget>().HasKey(b => b.IdBudget);
    builder.Entity<Budget>().Property(b => b.MontantLimite).HasPrecision(18, 2);
    builder.Entity<Budget>().HasOne(b => b.User).WithMany().HasForeignKey(b => b.IdUser).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<Budget>().HasOne(b => b.Category).WithMany().HasForeignKey(b => b.IdCategorie).OnDelete(DeleteBehavior.Restrict);

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