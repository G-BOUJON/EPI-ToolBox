using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Data;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.Periodic;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
{
    options.Cookie.Name = "CookieAuth";
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddDbContext<LicenseManagerDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LicenseManagerDB")), ServiceLifetime.Singleton);

builder.Services.AddSingleton<IConfigurationHandlerFactory, ConfigurationHandlerFactory>();
builder.Services.AddSingleton<IMFilesUsersHandlerFactory, MFilesAccountHandlerFactory>();
builder.Services.AddSingleton<IAccountsHistoryHandlerFactory, AccountsHistoryHandlerFactory>();

// builder.Services.AddSingleton<IAccountsListHandlerFactory, AccountsListHandlerFactory>();
builder.Services.AddSingleton<IAccountsListHandlerFactory, DbAccountsServiceFactory>();

builder.Services.AddSingleton<IADUsersHandlerFactory, ActiveDirectoryUserHandlerFactory>();

builder.Services.AddSingleton<IPeriodicOperations,PeriodicLicenseMangerJob>();
builder.Services.AddHostedService<LicenseMangerOperationHostedService>();


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

app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "licenseManager_route",
    areaName: "LicenseManager",
    pattern: "LicenseManager/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default_route",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
