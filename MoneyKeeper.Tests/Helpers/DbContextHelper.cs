using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyKeeper.Tests.Helpers;

public class DbContextHelper
{
    public static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
