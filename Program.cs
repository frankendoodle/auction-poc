using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using AuctionPoc.Components;
using AuctionPoc.Services.Auth;
using AuctionPoc.Data;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("AuctionDatabase");
if (!string.IsNullOrEmpty(connectionString))
{
    var password = builder.Configuration["auction-admin"];
    if (!string.IsNullOrEmpty(password))
    {
        connectionString = connectionString.Replace("{KEYVAULT-PASSWORD}", password);
    }
}
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<AuctionPoc.Services.AuctionService>();

builder.Services.AddSingleton<IUserStore, JsonUserStore>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsStaff", p => p.RequireRole(Roles.Staff));
    options.AddPolicy("IsAgentBuyer", p => p.RequireRole(Roles.AgentBuyer));
    options.AddPolicy("IsAgentSeller", p => p.RequireRole(Roles.AgentSeller));
    options.AddPolicy("IsAuctionClient", p => p.RequireRole(Roles.AuctionClient));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AuctionDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapPost("/auth/register", async (IUserStore store, [Microsoft.AspNetCore.Mvc.FromForm] string email, [Microsoft.AspNetCore.Mvc.FromForm] string password, [Microsoft.AspNetCore.Mvc.FromForm] string displayName, [Microsoft.AspNetCore.Mvc.FromForm] string role) =>
{
    var existing = await store.GetByEmailAsync(email);
    if (existing is not null) return Results.BadRequest("Email already registered.");
    if (string.IsNullOrWhiteSpace(role)) return Results.BadRequest("Role is required.");

    var rec = new UserRecord
    {
        Email = email,
        DisplayName = displayName,
        Role = role,
        PasswordHash = JsonUserStore.Hash(password)
    };
    var ok = await store.AddAsync(rec);
    if (!ok) return Results.BadRequest("Unable to register.");
    return Results.Ok();
});

app.MapPost("/auth/login", async (IUserStore store, HttpContext ctx, [Microsoft.AspNetCore.Mvc.FromForm] string email, [Microsoft.AspNetCore.Mvc.FromForm] string password, [Microsoft.AspNetCore.Mvc.FromForm] string? returnUrl) =>
{
    if (!await store.ValidateCredentialsAsync(email, password))
        return Results.BadRequest("Invalid credentials.");

    var rec = await store.GetByEmailAsync(email);
    if (rec is null) return Results.BadRequest("User not found.");

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, rec.Email),
        new(ClaimTypes.Name, rec.DisplayName),
        new(ClaimTypes.Email, rec.Email),
        new(ClaimTypes.Role, rec.Role)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    var redirect = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl!;
    return Results.Ok(new { redirect });
});

app.MapPost("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
