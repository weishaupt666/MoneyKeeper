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
}
