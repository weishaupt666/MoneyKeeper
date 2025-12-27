using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using MoneyKeeper.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<MoneyKeeper.Middleware.ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthorization();

app.Run();
