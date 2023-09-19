using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZeldaWebsite.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ZeldaWebsiteContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ZeldaWebsiteContext") ?? throw new InvalidOperationException("Connection string 'ZeldaWebsiteContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSession();

app.Run();
