using MoneyKeeper.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<MoneyKeeper.Middleware.ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthorization();

app.Run();
