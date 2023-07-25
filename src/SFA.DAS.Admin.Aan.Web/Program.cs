using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.AppStart;
using SFA.DAS.Admin.Aan.Web.Authentication;

var builder = WebApplication.CreateBuilder(args);

var rootConfiguration = builder.Configuration.LoadConfiguration();

builder.Services
    .AddOptions()
    .AddLogging()
    .AddApplicationInsightsTelemetry()
    .AddAuthenticationServices(rootConfiguration)
    .AddHttpContextAccessor()
    .AddSession(rootConfiguration)
    .AddServiceRegistrations(rootConfiguration)
    .AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddHealthChecks();

// Add services to the container.
builder.Services
    .Configure<RouteOptions>(o => o.LowercaseUrls = true)
    .AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
    .AddSessionStateTempDataProvider();

#if DEBUG
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif

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
