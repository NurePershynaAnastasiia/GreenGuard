using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews();
/*
builder.Services.AddDbContext<GreenGuardDbContext>(options =>
    options.UseSqlServer("Server=DESKTOP-D0GBIS9;Database=GreenGuard;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddDbContext<GreenGuardDbContext>(options =>
    options.UseSqlServer("Server=DESKTOP-D0GBIS9;Database=GreenGuard;Trusted_Connection=True;TrustServerCertificate=True;")
           .LogTo(Console.WriteLine, LogLevel.Information));

*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
