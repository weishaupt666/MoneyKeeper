using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;

namespace MoneyKeeper.Controllers;

public class TransactionController
{
    private readonly ApplicationDbContext _context;
    public TransactionController(ApplicationDbContext context)
    {
        _context = context;
    }
}
