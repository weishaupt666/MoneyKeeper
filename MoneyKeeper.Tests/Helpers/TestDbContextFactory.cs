using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyKeeper.Tests.Helpers;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        return context;
    }
}
