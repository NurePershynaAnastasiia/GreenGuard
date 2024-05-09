using GreenGuard.BuildInjections;
using GreenGuard.Data;
using GreenGuard.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using GreenGuard.BuildInjections;
using GreenGuard.Helpers;
using Microsoft.SqlServer.Management.Smo.Wmi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GreenGuardDbContext>(options =>
    options.UseSqlServer("Server=DESKTOP-D0GBIS9;Database=GreenGuard;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddScoped<IPasswordHasher<WorkerDto>, PasswordHasher<WorkerDto>>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:7223")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("CultureInvariant");

});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerWithJwtAuthorization();

builder.Services.AddLogging();
builder.Services.AddSetSecurity(builder.Configuration);

builder.Services.AddServices();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenGuard V1");
        c.RoutePrefix = "";
        c.OAuthClientId("swagger");
        c.OAuthAppName("Swagger UI");
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();