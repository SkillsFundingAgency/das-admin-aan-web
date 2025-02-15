using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.Admin.Aan.Web.AppStart;
using SFA.DAS.Admin.Aan.Web.Authentication;
using SFA.DAS.Admin.Aan.Web.Configuration;
using SFA.DAS.Admin.Aan.Web.Filters;
using SFA.DAS.Validation.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);

var rootConfiguration = builder.Configuration.LoadConfiguration();

builder.Services.AddOptions();

var applicationConfiguration = rootConfiguration.GetSection(nameof(ApplicationConfiguration)).Get<ApplicationConfiguration>()!;
builder.Services.Configure<ApplicationConfiguration>(rootConfiguration.GetSection(nameof(applicationConfiguration)));

builder.Services
    .AddLogging(options =>
    {
        options.AddApplicationInsights();
        options.AddFilter<ApplicationInsightsLoggerProvider>("SFA.DAS", LogLevel.Information);
        options.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
    })
    .AddApplicationInsightsTelemetry()
    .AddAuthenticationServices(rootConfiguration)
    .AddHttpContextAccessor()
    .AddDataProtection(rootConfiguration)
    .AddSession(rootConfiguration)
    .AddServiceRegistrations(rootConfiguration)
    .AddValidatorsFromAssembly(typeof(Program).Assembly)
    .Configure<RouteOptions>(o => o.LowercaseUrls = true)
    .AddControllersWithViews(options =>
    {
        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
        options.Filters.Add<RequiresEventSessionModelAttribute>();
        options.Filters.Add<RequiresMemberActionAttribute>();
        options.Filters.Add<ValidateModelStateFilter>();
    })
    .AddSessionStateTempDataProvider();
builder.Services.AddDasHealthChecks(applicationConfiguration);

builder.Services.AddFluentValidationAutoValidation();

#if DEBUG
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseContentSecurityPolicy();

app.Use(async (context, next) =>
{
    if (context.Response.Headers.ContainsKey("X-Frame-Options"))
    {
        context.Response.Headers.Remove("X-Frame-Options");
    }

    context.Response.Headers!.Append("X-Frame-Options", "SAMEORIGIN");

    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        //Re-execute the request so the user gets the error page
        var originalPath = context.Request.Path.Value;
        context.Items["originalPath"] = originalPath;
        context.Request.Path = "/error/404";
        await next();
    }
});

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseCookiePolicy()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseSession()
    .UseDasHealthChecks()
    .UseHealthChecks("/ping", new HealthCheckOptions
    {
        Predicate = (_) => false,
        ResponseWriter = (context, report) =>
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("");
        }
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
