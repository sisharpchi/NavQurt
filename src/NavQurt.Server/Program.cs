using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities;
using NavQurt.Core.Enumerations;
using NavQurt.Infrastructure;
using NavQurt.Server.Extensions;
using NavQurt.Server.HostedServices;
using NavQurt.Server.Services;
using NavQurt.Shared.Claims;
using NavQurt.Shared.Policies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationOptions(builder.Configuration);

builder.Services.AddInfrastructureLayer(
    builder.Configuration,
    optionsAction: options =>
    {
        options.UseOpenIddict<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, long>();
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(36);
    options.Cookie.MaxAge = options.ExpireTimeSpan;
    options.SlidingExpiration = true;
});

builder.Services.AddControllers();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Admin", "/", Permissions.SuperAdmin);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Permissions.SuperAdmin, policy => policy.RequireRole(AppRoles.SuperAdmin));

    foreach (var claim in ClaimsStore.AllClaims)
    {
        options.AddPolicy(claim.Claim.Value, policy => policy.RequireClaim(claim.Claim.Type, claim.Claim.Value));
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsapp", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.RegisterOpenIddict();
builder.Services.AddScoped<TokenRevocationService>();
builder.Services.AddHostedService<IdentitySeederService>();
builder.Services.AddHostedService<OpenIddictSeederService>();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddOpenTelemetryEx();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapPrometheusScrapingEndpoint();
app.MapControllers();
app.MapRazorPages();

app.Run();
