using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.ActiveDirectory.Sync;
using ToolBox_MVC.Services.MFiles;
using ToolBox_MVC.Services.MFiles.Connector;
using ToolBox_MVC.Services.MFiles.Sync;
using ToolBox_MVC.Services.Periodic;

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

builder.Services.AddDbContext<ToolBoxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ToolBoxDb")), ServiceLifetime.Scoped);


builder.Services.AddScoped<IPeriodicOperations,LicenseManagerPeriodicOperations>();
builder.Services.AddHostedService<LicenseMangerOperationHostedService>();

builder.Services.AddScoped<IMFilesConnectorFactory, MFConnectorFactory>();
builder.Services.AddScoped<IAdConnectorFactory, AdConnectorFactory>();
builder.Services.AddScoped<ValidateCredentialFactory>();

builder.Services.AddScoped<IMfilesAccountActivationHandler, MfAccountActivationService>();

builder.Services.AddScoped<IAdService,ActiveDirectoryService>();

builder.Services.AddScoped<IMFilesService, MFilesService>();

builder.Services.AddScoped<IMfCredentialStore, MFilesCredentialStore>();
builder.Services.AddScoped<IMfSyncService, MfSyncService>();
builder.Services.AddScoped<IActiveDirectorySyncService, ActiveDirectorySyncService>();


builder.Services.AddScoped<IADCredentialService, ADCredentialStore>();
builder.Services.AddScoped<ILicenseMangagerService, LicenseManager>();
builder.Services.AddScoped<IOperationHistoryService, OperationHistoryService>();

builder.Services.AddScoped<IGenericRepository<ActiveDirectory>, GenericRepository<ActiveDirectory>>();
builder.Services.AddScoped<IServerRepository, ServerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IHistoryOperationRepository, HistoryOperationRepository>();
builder.Services.AddScoped<IADAccountRepository, ADAccountRepository>();
builder.Services.AddScoped<IADGroupRepository, ADGroupRepository>();

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
    await ServerSeedData.Initialize(services);
}

    app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "LM_Home",
    areaName: "LicenseManager",
    pattern: "MFiles/Home",
    defaults: new {area = "LicenseManager",controller = "Home", action="Index"});
//pattern: "LicenseManager/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "LM_ServerDahsboard",
    areaName: "LicenseManager",
    pattern: "MFiles/{serverName}",
    defaults : new {controller = "Home", action="Details"});

app.MapAreaControllerRoute(
    name: "LM_ServerSpecific",
    areaName: "LicenseManager",
    pattern: "MFiles/{serverName}/{controller}/{action}",
    defaults : new {area = "LicenseManager", controller = "Home", action="Index"});


app.MapControllerRoute(
    name: "default_route",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
