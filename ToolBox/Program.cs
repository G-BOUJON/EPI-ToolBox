using ToolBox.Services.LicenseManagerTest;
using ToolBox.Services.LicenseManagerCert;
using ToolBox.Services.LicenseManagerProd;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ToolBox.Services.LicenseManagerTest.MFilesUsersService>();
builder.Services.AddTransient<ToolBox.Services.LicenseManagerCert.MFilesUsersService>();
builder.Services.AddTransient<ToolBox.Services.LicenseManagerProd.MFilesUsersService>();
builder.Services.AddTransient<ToolBox.Services.LicenseManagerTest.JsonConfService>();
builder.Services.AddTransient<ToolBox.Services.LicenseManagerCert.JsonConfService>();
builder.Services.AddTransient<ToolBox.Services.LicenseManagerProd.JsonConfService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
{
    options.Cookie.Name = "CookieAuth";
    options.LoginPath = "/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddScoped<ToolBox.Services.LicenseManagerTest.SampleService>();
builder.Services.AddSingleton<ToolBox.Services.LicenseManagerTest.PeriodicHostedService>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<ToolBox.Services.LicenseManagerTest.PeriodicHostedService>());

builder.Services.AddScoped<ToolBox.Services.LicenseManagerCert.SampleService>();
builder.Services.AddSingleton<ToolBox.Services.LicenseManagerCert.PeriodicHostedService>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<ToolBox.Services.LicenseManagerCert.PeriodicHostedService>());

builder.Services.AddScoped<ToolBox.Services.LicenseManagerProd.SampleService>();
builder.Services.AddSingleton<ToolBox.Services.LicenseManagerProd.PeriodicHostedService>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<ToolBox.Services.LicenseManagerProd.PeriodicHostedService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
