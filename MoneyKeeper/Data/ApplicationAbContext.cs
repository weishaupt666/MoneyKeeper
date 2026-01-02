using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Models;
using System.Collections.Generic;

namespace MoneyKeeper.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Products", UserId = null },
            new Category { Id = 2, Name = "Housing", UserId = null },
            new Category { Id = 3, Name = "Transport", UserId = null },
            new Category { Id = 4, Name = "Entertainment", UserId = null },
            new Category { Id = 5, Name = "Health", UserId = null },
            new Category { Id = 6, Name = "Salary", UserId = null }
        );

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(t => t.OriginalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(t => t.ExchangeRate).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.Property(w => w.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(w => w.CurrencyCode).HasDefaultValue("PLN").IsRequired();
        });
    }
}
