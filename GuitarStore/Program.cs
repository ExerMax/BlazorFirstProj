using BlazorStrap;
using ApplicationDbAccess;
using GuitarStore.Log;
using GuitarStore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "Log/logger.txt"));

string connection = builder.Configuration.GetConnectionString("DefaultConnection");



// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/Login";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/access-denied";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection))
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddBlazorStrap();
builder.Services.AddFeatureManagement();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GuitarStore.Client._Imports).Assembly);

//Test
app.MapGet("/minAPI/users", (ApplicationDbContext db) => JsonSerializer.Serialize(db.Users.ToList()));
app.MapGet("/minAPI/products", (ApplicationDbContext db) => JsonSerializer.Serialize(db.Products.ToList()));
app.Map("/minAPI/hello", (ILogger<Program> logger) =>
{
    logger.LogInformation($"Path: /hello  Time: {DateTime.Now.ToLongTimeString()}");
    return "Hello World";
});
app.MapGet("/minapi/login", async (HttpContext context) =>
{
    var claimsIdentity = new ClaimsIdentity("Undefined");
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    // установка аутентификационных куки
    await context.SignInAsync(claimsPrincipal);
    return Results.Redirect("/");
});

app.Run();