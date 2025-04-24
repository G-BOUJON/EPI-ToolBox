using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Data;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Data;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.MFiles;
using ToolBox_MVC.Services.MFiles.Connector;
using ToolBox_MVC.Services.MFiles.Sync;
using ToolBox_MVC.Services.Periodic;
using ToolBox_MVC.Services.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddDataProtection();

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
{
    options.Cookie.Name = "CookieAuth";
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddDbContext<LicenseManagerDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LicenseManagerDB")), ServiceLifetime.Singleton);
builder.Services.AddDbContext<ToolBoxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ToolBoxDb")), ServiceLifetime.Singleton);

builder.Services.AddSingleton<IConfigurationHandlerFactory, ConfigurationHandlerFactory>();
builder.Services.AddSingleton<IMFilesUsersHandlerFactory, MFilesAccountHandlerFactory>();
builder.Services.AddSingleton<IAccountsHistoryHandlerFactory, AccountsHistoryHandlerFactory>();

// builder.Services.AddSingleton<IAccountsListHandlerFactory, AccountsListHandlerFactory>();
builder.Services.AddSingleton<IAccountsListHandlerFactory, DbAccountsServiceFactory>();

builder.Services.AddSingleton<IADUsersHandlerFactory, ActiveDirectoryUserHandlerFactory>();

builder.Services.AddSingleton<IPeriodicOperations,PeriodicLicenseMangerJob>();
builder.Services.AddHostedService<LicenseMangerOperationHostedService>();

builder.Services.AddScoped<IMFilesConnectorFactory, MFConnectorFactory>();
builder.Services.AddScoped<IAdConnectorFactory, AdConnectorFactory>();

builder.Services.AddScoped<IAdService,ActiveDirectoryService>();

builder.Services.AddScoped<IMFilesService, MFilesService>();
builder.Services.AddScoped<IMfilesServerRepository, MFilesServerRepository>();
builder.Services.AddScoped<ICredentialRepository, MFilesCredentialStore>();
builder.Services.AddScoped<ISyncService, MFilesSyncService>();
builder.Services.AddScoped<IGroupAccountRepository,MFilesAccountGroupRepository>();
builder.Services.AddScoped<IAccountsRepository, MFilesAccountsRepository>();
builder.Services.AddScoped<IGroupRepository, MFilesGroupsRepository>();
builder.Services.AddScoped<IActiveDirectoryUsersHandler, ADUserHandlerTest>(); //test class
builder.Services.AddScoped<IADCredRepository, ADCredentialStore>();
builder.Services.AddScoped<ILicenseMangagerService, LicenseManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    ServerSeedData.Initialize(services);
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
